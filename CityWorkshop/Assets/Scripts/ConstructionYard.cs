using UnityEngine;

public partial class TwinWorkshop
{
    void BuildYard(TwinWalker walker)
    {
        var yard = Node("Строительная площадка", transform, new Vector3(-27,0,0));
        Body("Уплотнённый грунт", yard.transform, new Vector3(0,.04f,0), new Vector3(19,.08f,32), new Color(.60f,.48f,.32f));
        var controls = yard.AddComponent<YardControls>();
        controls.walker = walker;
        controls.machines = new Rigidbody[4];
        controls.tools = new Transform[4];
        controls.tips = new Transform[4];
        var yellow = new Color(.95f,.59f,.09f);
        string[] names = {"Бульдозер","Экскаватор","Башенный кран","Грузовик"};
        Vector3[] positions = {new Vector3(-4,.12f,-8),new Vector3(2,.12f,-6),new Vector3(-5,.12f,6),new Vector3(3,.12f,4)};
        for (int i=0;i<4;i++) {
            var chassis = Node(names[i],yard.transform,positions[i]);
            var rb = chassis.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            controls.machines[i] = rb;
            if (i==2) {
                Body("Основание",chassis.transform,Vector3.up*.25f,new Vector3(2.6f,.5f,2.6f),Slate);
                Body("Мачта",chassis.transform,Vector3.up*3.3f,new Vector3(.5f,6.4f,.5f),yellow);
                var boom=Node("Поворотная стрела",chassis.transform,Vector3.up*6.4f);
                Body("Стрела",boom.transform,new Vector3(0,0,2),new Vector3(.35f,.4f,7),yellow);
                Body("Противовес",boom.transform,new Vector3(0,0,-1.6f),new Vector3(.85f,.8f,.9f),Slate);
                controls.tools[i]=boom.transform;
                controls.tips[i]=Body("Крюк",boom.transform,new Vector3(0,-5.6f,5),Vector3.one*.38f,Clay,PrimitiveType.Sphere).transform;
                controls.cable=Body("Трос",boom.transform,new Vector3(0,-2.8f,5),new Vector3(.045f,5.6f,.045f),Slate,PrimitiveType.Cube,false).transform;
                continue;
            }
            Body("Шасси",chassis.transform,Vector3.up*.65f,new Vector3(2.0f,.58f,3.1f),i==3?Teal:yellow);
            Body("Кабина",chassis.transform,new Vector3(0,1.35f,i==3?1.2f:-.7f),new Vector3(1.3f,1.05f,1.1f),Teal);
            for (int a=-1;a<=1;a+=2) {
                if (i==3) {
                    foreach(float z in new[]{-1f,1f})
                        Body("Колесо",chassis.transform,new Vector3(a,.42f,z),new Vector3(.8f,.18f,.8f),Slate,PrimitiveType.Cylinder).transform.localRotation=Quaternion.Euler(0,0,90);
                } else Body("Гусеница",chassis.transform,new Vector3(a*.9f,.32f,0),new Vector3(.45f,.56f,3.0f),Slate);
            }
            var tool=Node("Рабочий орган",chassis.transform,i==3?new Vector3(0,.94f,-1.4f):new Vector3(0,.87f,1.1f));
            controls.tools[i]=tool.transform;
            if(i==0) {
                controls.tips[i]=Body("Отвал",tool.transform,new Vector3(0,-.34f,.88f),new Vector3(2.9f,.93f,.22f),yellow).transform;
            } else if(i==1) {
                Body("Стрела",tool.transform,new Vector3(0,.59f,.95f),new Vector3(.34f,.36f,2.2f),yellow).transform.localRotation=Quaternion.Euler(-27,0,0);
                Body("Рукоять",tool.transform,new Vector3(0,.34f,2.0f),new Vector3(.26f,1.4f,.28f),yellow);
                var bucket=Body("Дно ковша",tool.transform,new Vector3(0,-.50f,2.25f),new Vector3(1.35f,.16f,.85f),Slate);
                Body("Спинка ковша",tool.transform,new Vector3(0,-.24f,1.9f),new Vector3(1.35f,.55f,.15f),Slate);
                controls.tips[i]=bucket.transform;
            } else {
                Body("Дно кузова",tool.transform,new Vector3(0,0,.65f),new Vector3(2.25f,.13f,2.15f),Teal);
                foreach(int sign in new[]{-1,1})
                    Body("Борт",tool.transform,new Vector3(sign*1.06f,.36f,.65f),new Vector3(.12f,.75f,2.15f),Teal);
                Body("Передний борт",tool.transform,new Vector3(0,.44f,1.67f),new Vector3(2.25f,.88f,.15f),Teal);
                controls.tips[i]=tool.transform;
            }
        }
        controls.stones=new Rigidbody[40];
        for(int i=0;i<40;i++) {
            var pos = new Vector3(-4f+(i%5)*.42f, .33f+(i/20)*.44f, -4f+(i/5%4)*.47f);
            var rock=Body("Камень "+(i+1),yard.transform,pos,Vector3.one*(.34f+(i%3)*.04f),i%2==0?Timber:Slate,PrimitiveType.Sphere);
            var rb=rock.AddComponent<Rigidbody>();rb.mass=.6f;rb.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;
            controls.stones[i]=rb;
        }
        var camera=Node("Обзор стройплощадки",yard.transform,new Vector3(13,14,-18)).AddComponent<Camera>();
        camera.transform.LookAt(yard.transform.position);
        camera.fieldOfView=52;camera.enabled=false;controls.overview=camera;
    }
}

