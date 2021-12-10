using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class PlayerCtr : MonoBehaviourPunCallbacks, IPunObservable
{
private CharacterController controller;
public float speed;
private new Camera camera;
private Ray ray;
private Plane plane;
private Vector3 hitPoint;



Vector3 moveVec;
Animator anim;
 //photonView 컴포넌트 캐시 처리를 위한 변수
 private PhotonView pv;
  //시네머신 가상 카메라를 저장할 변수
 private CinemachineVirtualCamera virtualCamera;

 //수신된 위치와 회저값을 저장할 변수
 private Vector3 receivePos;
 private Quaternion receiveRot;
 //수신된 좌표로의 이동 및 회전 속도의 민감도
 public float damping = 10.0f;
void Start()
{
    controller = GetComponent<CharacterController>();
    anim = GetComponentInChildren<Animator>();
    camera =Camera.main;
    

    pv = GetComponent<PhotonView>();
    virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

    //photonView가 자신의 것일 경우 시네머신 가상카메라를 연결
    if(pv.IsMine)
    {
        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;
    }


    plane = new Plane(transform.up,transform.position);


}
void Update()
{
    if(pv.IsMine)
    {
         Move();
         turn();
    }
    else
    {
        //수신된 좌표로 보간한 이동처리
        transform.position =Vector3.Lerp(transform.position,
                                         receivePos,
                                         Time.deltaTime*damping);
        //수신된 회전값으로 보간한 회전 처리
        transform.rotation =Quaternion.Slerp(transform.rotation,
                                       receiveRot,
                                         Time.deltaTime*damping);
    }
}
  float hAxis => Input.GetAxis("Horizontal");
  float  vAxis => Input.GetAxis("Vertical");
    void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

         //이동할 방향 벡터 계산
         Vector3 moveDir = (cameraForward * vAxis) + (cameraRight * hAxis);
         moveDir.Set(moveDir.x, 0.0f, moveDir.z);

        //주인공 캐릭터 이동 처리
         controller.SimpleMove(moveDir.normalized * speed);
        // Debug.Log("Horizontal: "+hAxis);
         //Debug.Log("Vertical: "+vAxis);

         anim.SetBool("isWalk", moveDir !=Vector3.zero);

        transform.LookAt(transform.position + moveDir);
    }
    void turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        plane.Raycast(ray,out enter);
        hitPoint = ray.GetPoint(enter);
        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y=0;
        transform.localRotation = Quaternion.LookRotation(lookDir);
    }

    public void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        //자신의 로컬 캐릭터인 경우 자신의 테이터를 다른 네트워크 유저에게 송신
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
