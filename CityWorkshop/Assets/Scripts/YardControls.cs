using UnityEngine;

public class YardControls : MonoBehaviour
{
    public Rigidbody[] machines;
    public Transform[] tools, tips;
    public Rigidbody[] stones;
    public Transform cable;
    public TwinWalker walker;
    public Camera overview;
    public int ActiveMachine { get; private set; } = -1;
    Rigidbody load;
    float drive, steering, lift;

    void Start() { SelectMachine(0); }

    public void SelectMachine(int index)
    {
        if(index < -1 || index >= machines.Length) return;
        Release();
        drive=steering=lift=0;
        ActiveMachine=index;
        overview.enabled=index>=0;
        walker.eyes.enabled=index<0;
        walker.enabled=index<0;
        Cursor.lockState=CursorLockMode.None;
    }

    void Release()
    {
        if(!load) return;
        load.transform.SetParent(transform,true);
        load.isKinematic=false;
        load.detectCollisions=true;
        load.WakeUp();
        load=null;
    }

    public void ToggleLoad()
    {
        if(load) { Release(); return; }
        if(ActiveMachine!=1 && ActiveMachine!=2) return;
        Transform tip=tips[ActiveMachine];
        float distance=ActiveMachine==1?1.15f:.85f;
        Rigidbody nearest=null;
        foreach(var stone in stones) {
            float candidate=Vector3.Distance(stone.position,tip.position);
            if(candidate<distance) { distance=candidate; nearest=stone; }
        }
        if(!nearest) return;
        load=nearest;
        load.isKinematic=true;
        load.detectCollisions=false;
        load.transform.SetParent(tip,true);
        load.transform.position=tip.position+Vector3.up*(ActiveMachine==1?.32f:-.32f);
    }

    void Update()
    {
        for(int i=0;i<4;i++)
            if(Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1+i))) SelectMachine(i);
        if(Input.GetKeyDown(KeyCode.Tab)) SelectMachine(-1);
        if(ActiveMachine<0) return;
        drive=Input.GetAxisRaw("Vertical");
        steering=Input.GetAxisRaw("Horizontal");
        lift=(Input.GetKey(KeyCode.R)?1:0)-(Input.GetKey(KeyCode.F)?1:0);
        if(Input.GetKeyDown(KeyCode.T)) ToggleLoad();
    }

    void FixedUpdate()
    {
        if(ActiveMachine>=0) Step(drive,steering,lift,Time.fixedDeltaTime);
    }

    public void Step(float forward,float turn,float lever,float dt)
    {
        int index=ActiveMachine;
        if(index<0) return;
        var rb=machines[index];
        Transform joint=tools[index];
        if(index==2) {
            joint.Rotate(0,turn*40*dt,0,Space.Self);
            var hook=tips[index];
            Vector3 local=hook.localPosition;
            local.z=Mathf.Clamp(local.z+forward*2*dt,.9f,5.2f);
            local.y=Mathf.Clamp(local.y+lever*2.5f*dt,-6.08f,-.65f);
            hook.localPosition=local;
            cable.localPosition=new Vector3(0,local.y/2,local.z);
            cable.localScale=new Vector3(.045f,-local.y,.045f);
            return;
        }
        Vector3 limit=rb.position+rb.transform.forward*forward*3.2f*dt-transform.position;
        limit.x=Mathf.Clamp(limit.x,-8,8);
        limit.z=Mathf.Clamp(limit.z,-14,14);
        rb.MovePosition(transform.position+limit);
        rb.MoveRotation(rb.rotation*Quaternion.Euler(0,turn*47*dt,0));
        if(index==0) {
            var at=joint.localPosition;
            at.y=Mathf.Clamp(at.y+lever*dt,.80f,1.65f);
            joint.localPosition=at;
        } else {
            float angle=Mathf.DeltaAngle(0,joint.localEulerAngles.x);
            joint.localRotation=Quaternion.Euler(Mathf.Clamp(angle-lever*30*dt,index==1?-63:-43,index==1?8:0),0,0);
        }
    }

    void OnGUI()
    {
        if(ActiveMachine<0) return;
        GUI.Box(new Rect(12,12,550,70),
            machines[ActiveMachine].name+"   |   1–4: смена машины   |   Tab: пешком\n"+
            "W/S: ход или вылет крюка   A/D: поворот   R/F: поднять или опустить\n"+
            "T: взять или отпустить камень у ковша / крюка");
    }
}

