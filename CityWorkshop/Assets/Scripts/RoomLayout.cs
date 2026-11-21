using UnityEngine;

public partial class TwinWorkshop
{
    void BuildRoom()
    {
        var room = Node("Комната мастерской",transform,new Vector3(0,0,70)).transform;
        Body("Пол",room,new Vector3(0,-.15f,0),new Vector3(12,.3f,12),Timber);
        Body("Потолок",room,new Vector3(0,3.65f,0),new Vector3(12,.2f,12),Ivory);
        Body("Северная стена",room,new Vector3(0,1.8f,6),new Vector3(12,3.6f,.2f),Ivory);
        Body("Западная стена",room,new Vector3(-6,1.8f,0),new Vector3(.2f,3.6f,12),Ivory);
        Body("Южная стена",room,new Vector3(0,1.8f,-6),new Vector3(12,3.6f,.2f),Ivory);
        Body("Восточная стена слева",room,new Vector3(6,1.8f,-3.95f),new Vector3(.2f,3.6f,4.1f),Ivory);
        Body("Восточная стена справа",room,new Vector3(6,1.8f,3.95f),new Vector3(.2f,3.6f,4.1f),Ivory);
        Body("Подоконник",room,new Vector3(6,.55f,0),new Vector3(.35f,1.1f,3.8f),Ivory);
        Body("Перемычка",room,new Vector3(6,3.3f,0),new Vector3(.22f,.6f,3.8f),Ivory);
        Body("Окно",room,new Vector3(6.03f,2.05f,0),new Vector3(.06f,1.9f,3.8f),new Color(.45f,.69f,.75f));
        var ceiling=Lamp("Потолочный свет",room,LightType.Point,new Vector3(0,3.2f,0),new Color(1,.86f,.66f),4.4f,14);
        var toggle=Body("Выключатель света",room,new Vector3(-5.82f,1.35f,-1.8f),new Vector3(.18f,.32f,.28f),Clay);
        var lightSwitch=toggle.AddComponent<RoomDevice>(); lightSwitch.kind=RoomDevice.Kind.Lamp;
        lightSwitch.lamp=ceiling; lightSwitch.active=true;lightSwitch.movingParts=new Transform[0];

        Furniture(room,"Стол",new Vector3(-.2f,0,1.3f),2.4f,1.2f,.94f,12);
        Furniture(room,"Письменный стол",new Vector3(2.7f,0,4.5f),1.9f,1.1f,.94f,14);
        Furniture(room,"Стул",new Vector3(-.35f,0,-.1f),.65f,.65f,.46f,4,true);
        Furniture(room,"Стул у стены",new Vector3(3.1f,0,3.2f),.65f,.65f,.46f,4,true);
        Pickup(room,"Карандашница",new Vector3(-.3f,1.18f,1.3f),new Vector3(.2f,.25f,.2f),Teal,.16f);
        Pickup(room,"Брусок",new Vector3(.38f,1.15f,1.45f),new Vector3(.32f,.15f,.20f),Clay,.22f);
        Pickup(room,"Кубик на полу",new Vector3(1.8f,.18f,-1.1f),Vector3.one*.3f,Clay,.3f);
        var cabinet=Node("Шкаф",room,new Vector3(-2.1f,0,4.5f)).transform;
        Body("Задняя стенка",cabinet,new Vector3(0,1.05f,.4f),new Vector3(1.7f,2.1f,.12f),Timber);
        for(int s=-1;s<=1;s+=2) Body("Боковина",cabinet,new Vector3(s*.82f,1.05f,0),new Vector3(.12f,2.1f,.88f),Timber);
        for(int i=0;i<3;i++) Body("Полка",cabinet,new Vector3(0,.1f+i*.92f,0),new Vector3(1.6f,.09f,.85f),Timber);
        RoomDevice cabinetDoor=null;
        if (lesson>=11) cabinetDoor=Hinge(cabinet,"Дверь шкафа",new Vector3(-.81f,0,-.49f),1.62f,2.02f,92);
        Pickup(cabinet,"Коробка в шкафу",new Vector3(0,1.19f,0),Vector3.one*.24f,Teal,.28f,cabinetDoor);
        Portal("Вход в мастерскую",transform,new Vector3(3.7f,0,-4),new Vector3(-4.4f,.04f,68.4f));
        // При установленной двери выход находится за ней в нише.
        float exitZ=lesson>=11?-4.8f:-3.8f;
        Portal("Выход к станции",room,new Vector3(-4.5f,0,exitZ),new Vector3(3.7f,.25f,-6.4f));
        if (lesson<11) return;
        Body("Перегородка слева",room,new Vector3(-5.65f,1.45f,-3.6f),new Vector3(.7f,2.9f,.18f),Ivory);
        Body("Перегородка справа",room,new Vector3(-1.72f,1.45f,-3.6f),new Vector3(4.0f,2.9f,.18f),Ivory);
        Hinge(room,"Входная дверь",new Vector3(-5.29f,0,-3.6f),1.53f,2.5f,-100);
        var fan=Node("Настольный вентилятор",room,new Vector3(-1.9f,0,1.3f)).transform;
        Body("Основание",fan,new Vector3(0,.08f,0),new Vector3(.6f,.14f,.6f),Slate);
        Body("Стойка",fan,new Vector3(0,.65f,0),new Vector3(.12f,1.20f,.12f),Teal);
        var hub=Body("Ступица",fan,new Vector3(0,1.28f,0),Vector3.one*.28f,Clay,PrimitiveType.Sphere);
        var spin=Node("Крыльчатка",hub.transform,Vector3.zero).transform;
        // Компенсация масштаба ступицы оставляет геометрию лопастей в метрах.
        spin.localScale=Vector3.one/.28f;
        for(int i=0;i<4;i++){
            var arm=Node("Лопасть",spin,Vector3.zero).transform;arm.localRotation=Quaternion.Euler(0,0,90*i);
            Body("Перо",arm,new Vector3(0,.27f,0),new Vector3(.15f,.46f,.06f),Teal,PrimitiveType.Cube,false);
        }
        fan.localRotation=Quaternion.Euler(0,90,0);
        var blower=fan.gameObject.AddComponent<RoomDevice>();blower.kind=RoomDevice.Kind.Fan;
        blower.movingParts=new[]{spin};blower.windOrigin=fan.position+Vector3.up*1.25f;blower.windDirection=Vector3.right;
        var blinds=Node("Жалюзи",room,new Vector3(5.77f,0,0)).transform;
        blinds.localRotation=Quaternion.Euler(0,90,0);
        var lever=Body("Ручка жалюзи",blinds,new Vector3(-1.85f,1.25f,0),new Vector3(.12f,.36f,.16f),Clay);
        var mechanism=blinds.gameObject.AddComponent<RoomDevice>();mechanism.kind=RoomDevice.Kind.Blinds;
        mechanism.movingParts=new Transform[8];
        for(int i=0;i<8;i++) mechanism.movingParts[i]=Body("Ламель",blinds,new Vector3(0,1.28f+i*.22f,0),
            new Vector3(3.5f,.20f,.055f),Teal).transform;
    }
    RoomDevice Hinge(Transform parent,string label,Vector3 at,float width,float height,float angle)
    {
        var joint=Node(label,parent,at);
        var hinge=Node("Ось",joint.transform,Vector3.zero).transform;
        Body("Полотно",hinge,new Vector3(width/2,height/2,0),new Vector3(width,height,.1f),Teal);
        Body("Ручка",hinge,new Vector3(width-.18f,height*.47f,-.09f),new Vector3(.18f,.07f,.16f),Clay);
        var device=joint.AddComponent<RoomDevice>();device.kind=RoomDevice.Kind.Door;
        device.angle=angle;device.movingParts=new[]{hinge};return device;
    }
    void Portal(string name,Transform parent,Vector3 at,Vector3 destination)
    {
        var gate=Node(name,parent,at);
        for(int s=-1;s<=1;s+=2)
            Body("Стойка портала",gate.transform,new Vector3(s*.78f,1.22f,0),new Vector3(.16f,2.44f,.35f),Teal);
        Body("Сигнальная балка",gate.transform,new Vector3(0,2.43f,0),new Vector3(1.72f,.14f,.35f),Clay);
        var trigger=gate.AddComponent<BoxCollider>(); trigger.center=Vector3.up*1.1f;
        trigger.size=new Vector3(1.30f,2.2f,.34f);trigger.isTrigger=true;
        var rb=gate.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
        gate.AddComponent<PortalGate>().destination=destination;
    }
    void Furniture(Transform parent,string label,Vector3 at,float w,float d,float h,float mass,bool back=false)
    {
        var root=Node(label,parent,at); var p=root.transform;
        Body("Столешница или сиденье",p,new Vector3(0,h,0),new Vector3(w,.11f,d),Timber);
        for(int ix=-1;ix<=1;ix+=2) for(int iz=-1;iz<=1;iz+=2)
            Body("Ножка",p,new Vector3(ix*(w*.5f-.08f),h/2,iz*(d*.5f-.08f)),new Vector3(.1f,h,.1f),Slate);
        if (back) Body("Спинка",p,new Vector3(0,h+.27f,d*.45f),new Vector3(w,.54f,.08f),Teal);
        var rb=root.AddComponent<Rigidbody>();rb.mass=mass;rb.linearDamping=1.1f;rb.angularDamping=1.7f;
        rb.constraints=RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ;
    }
    void Pickup(Transform parent,string name,Vector3 at,Vector3 size,Color color,float mass,RoomDevice cabinet=null)
    {
        var g=Body(name,parent,at,size,color);
        var rb=g.AddComponent<Rigidbody>();rb.mass=mass;rb.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;
        g.AddComponent<CarryItem>().cabinet=cabinet;
    }
}
