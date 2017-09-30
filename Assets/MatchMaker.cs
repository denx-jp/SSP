using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MatchMaker : MonoBehaviour
{
    void Start()
    {
        NetworkManager.singleton.StartMatchMaker();
        
    }

    //call this method to request a match to be created on the server
    public void CreateInternetMatch(string matchName)
    {
        uint playerCount = 6;
        var matchAdvertise = true;          //NetworkMatch.ListMatchesで帰ってくるList<MatchInfoSnapshot>に、このマッチを含めるかどうか
        var matchPassword = "";
        var publicClientAddress = "";       //クライアントがインターネット経由で直接接続するためのネットワークアドレス
        var privateClientAddress = "";     //クライアントが LAN 経由で直接接続するためのネットワークアドレス
        var eloScoreForMatch = 0;          //いわゆるスキルレート。全クライアントが0だとランダムになる
        var requestDomain = 0;              //クライアントバージョンを区別するための番号

        NetworkManager.singleton.matchMaker.CreateMatch(matchName, playerCount, matchAdvertise, matchPassword, publicClientAddress, privateClientAddress, eloScoreForMatch, requestDomain, OnInternetMatchCreate);
    }

    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(hostInfo, 9000);

            NetworkManager.singleton.StartHost(hostInfo);
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }
    
    public void FindInternetMatch(string matchName)
    {
        var startPageNumber = 0;                                    //リストし始めるページ
        var resultPageSize = 10;                                       //callbackに渡すリストのマッチの最大数
        var matchNameFilter = matchName;                   //*<matchNameFilter>*に該当するマッチが検索される
        var filterOutPrivateMatchesFromResults = true;   //プライベートマッチを検索結果に含めるかどうか
        var eloScoreTarget = 0;                                         //検索するときのスキルレート
        var requestDomain = 0;                                        //クライアントバージョンを区別するための番号

        NetworkManager.singleton.matchMaker.ListMatches(startPageNumber, resultPageSize, matchNameFilter, filterOutPrivateMatchesFromResults, eloScoreTarget, requestDomain, OnInternetMatchList);
    }

    //this method is called when a list of matches is returned
    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
                NetworkManager.singleton.matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
            else
                Debug.Log("No matches in requested room!");
        }
        else
        {
            Debug.LogError("Couldn't connect to match maker");
        }
    }

    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            MatchInfo hostInfo = matchInfo;
            NetworkManager.singleton.StartClient(hostInfo);
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }
}
