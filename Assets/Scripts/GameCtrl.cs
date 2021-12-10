using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;

//

public class GameCtrl : MonoBehaviourPunCallbacks
{
    public TMP_Text roomName;
    public TMP_Text connectInfo;
    public TMP_Text msgList;
    public Button exitBtn;
 void Awake()
 {
     CreatePlayer();
     //접속 정보 추출 및 표시
     SetRoomInfo();
     //Exit 버튼 이벤트 연셜
     exitBtn.onClick.AddListener(()=>OnExitClick());
 }
 void CreatePlayer()
 {
     //출현 위치 정보를 배열에 저장
     Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
     int idx = Random.Range(1,points.Length);

     //네트워크상에 캐릭터 생성
     PhotonNetwork.Instantiate("girls",points[idx].position,points[idx].rotation,0);
 }
 void SetRoomInfo()
 {
     Room room = PhotonNetwork.CurrentRoom;
     roomName.text = room.Name;
     connectInfo.text = $"({room.PlayerCount}/{room.MaxPlayers})";
 }
 //Exit 버튼의 OnClick에 연결할 함수
 private void OnExitClick()
 {
     PhotonNetwork.LeaveRoom();
 }
 //포톤 룸에서 퇴장했을 때 호출되는 콜백 함수
 public override void OnLeftRoom()
 {
     SceneManager.LoadScene("Lobby");
 }
 //룸으로 새로운 네트워크 유저가 접속했을 때 호출되는 콜백 함수
 public override void OnPlayerEnteredRoom(Player newPlayer)
{
    SetRoomInfo();
    string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room";
    msgList.text += msg;
}
//룸에서 네트워크 유저가 퇴장했을 때 호출되는 콜백 함수
public override void OnPlayerLeftRoom(Player otherPlayer)
{
    SetRoomInfo();
    string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> is left room";
    msgList.text += msg;
}
}
