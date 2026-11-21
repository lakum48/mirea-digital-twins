using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class TwinWorkshop : MonoBehaviour
{
    [Range(2, 16)] public int lesson = 11;
    readonly Dictionary<Color, Material> palette = new Dictionary<Color, Material>();
    Material mapped;
    static readonly Color Teal = new Color(.10f, .48f, .52f);
    static readonly Color Clay = new Color(.85f, .32f, .16f);
    static readonly Color Ivory = new Color(.84f,.81f,.68f);
    static readonly Color Slate = new Color(.19f,.24f,.28f);
    static readonly Color Timber = new Color(.48f,.29f,.15f);

    void Awake() {
        if (transform.childCount == 0) Build();
        if (lesson <= 3)
            for (int i = 1; i < Mathf.Min(3, Display.displays.Length); i++) Display.displays[i].Activate();
    }

    public void Build()
    {
        if (lesson <= 3) { ShapeLab(); return; }
        BuildCity();
        if (lesson >= 7) AddTraffic();
        if (lesson >= 9) BuildRoom();
        var player = Node("Пешеход", transform, new Vector3(3.7f,.25f,-10));
        var controller = player.AddComponent<CharacterController>();
        controller.height=1.8f; controller.radius=.32f; controller.center=Vector3.up*.9f;
        var eye = Node("Обзор пешехода", player.transform, new Vector3(0,1.62f,0)).AddComponent<Camera>();
        eye.tag="MainCamera"; eye.nearClipPlane=.06f; eye.fieldOfView=68;
        var walker = player.AddComponent<TwinWalker>(); walker.eyes=eye;
        if (lesson == 16) BuildYard(walker);
    }

    GameObject Node(string label, Transform parent, Vector3 local)
    {
        var g = new GameObject(label);
        g.transform.SetParent(parent,false); g.transform.localPosition = local;
        return g;
    }
    Material Paint(Color color)
    {
        if (!palette.TryGetValue(color, out Material m)) {
            m = new Material(Shader.Find("Standard"));
            m.color=color; m.name="Материал "+ColorUtility.ToHtmlStringRGB(color);
            palette[color]=m;
        }
        return m;
    }
    GameObject Body(string label, Transform parent, Vector3 at, Vector3 size, Color color,
                    PrimitiveType shape=PrimitiveType.Cube, bool solid=true)
    {
        var g = GameObject.CreatePrimitive(shape); g.name=label;
        g.transform.SetParent(parent,false); g.transform.localPosition=at; g.transform.localScale=size;
        g.GetComponent<Renderer>().sharedMaterial=Paint(color);
        if (!solid) g.GetComponent<Collider>().enabled=false;
        return g;
    }
    Light Lamp(string name, Transform parent, LightType type, Vector3 at, Color color, float power, float range=15)
    {
        var light=Node(name,parent,at).AddComponent<Light>();
        light.type=type; light.color=color; light.intensity=power; light.range=range; light.shadows=LightShadows.Soft;
        return light;
    }
    void Sky(float power) {
        RenderSettings.ambientMode=AmbientMode.Flat;
        RenderSettings.ambientLight=new Color(.24f,.26f,.29f);
        Lamp("Солнце",transform,LightType.Directional,new Vector3(8,14,-4),new Color(1,.94f,.83f),power)
            .transform.rotation=Quaternion.Euler(48,-35,0);
    }
    void ShapeLab()
    {
        Sky(.8f);
        Body("Площадка",transform,new Vector3(0,-.15f,0),new Vector3(18,.3f,12),new Color(.46f,.49f,.50f));
        var box=Body("Параллелепипед",transform,new Vector3(-5,1.0f,0),new Vector3(1.7f,2f,1),Ivory);
        var ellipsoid=Body("Эллипсоид",transform,new Vector3(-1.7f,.85f,.2f),new Vector3(2,1.7f,1.1f),Teal,PrimitiveType.Sphere);
        var cyl=Body("Цилиндр",transform,new Vector3(1.7f,1.2f,0),new Vector3(1.35f,1.2f,.95f),Clay,PrimitiveType.Cylinder);
        var capsule=Body("Капсула",transform,new Vector3(4.7f,1.5f,0),new Vector3(1.15f,1.5f,1.35f),Ivory,PrimitiveType.Capsule);
        capsule.transform.localRotation=Quaternion.Euler(0,0,12);
        Lamp("Холодный точечный",transform,LightType.Point,new Vector3(3.6f,2.5f,-2),new Color(.25f,.6f,1),4.0f,7);
        var spot=Lamp("Тёплый прожектор",transform,LightType.Spot,new Vector3(-4.5f,5,-3),new Color(1,.50f,.27f),6f,12);
        spot.spotAngle=52; spot.transform.LookAt(box.transform);
        CameraView("Общий вид",0,new Vector3(0,4.8f,-12.5f),new Vector3(0,.65f,0),60);
        CameraView("Точечный свет",1,new Vector3(6.6f,3.4f,-5.4f),new Vector3(3.0f,1f,0),53);
        CameraView("Прожектор",2,new Vector3(-6.8f,3.3f,-5.3f),new Vector3(-3.8f,.8f,0),53);
        if(lesson==3) {
            box.AddComponent<ShapeMove>();
            cyl.AddComponent<ShapeScale>();
            capsule.AddComponent<ShapeTurn>();
            ellipsoid.AddComponent<ShapeMove>();
            ellipsoid.AddComponent<ShapeScale>();
            ellipsoid.AddComponent<ShapeTurn>();
        }
    }
    void CameraView(string name,int display,Vector3 at,Vector3 target,float field)
    {
        var cam=Node(name,transform,at).AddComponent<Camera>();
        cam.transform.LookAt(target); cam.targetDisplay=display; cam.fieldOfView=field;
        cam.clearFlags=CameraClearFlags.SolidColor; cam.backgroundColor=new Color(.13f,.18f,.21f);
        if (display==0) cam.tag="MainCamera";
    }
    void ImportTile(string model,Vector3 at,float width,bool building)
    {
        var prefab=Resources.Load<GameObject>("City/"+model);
        if (!prefab) throw new System.InvalidOperationException("Нет модели "+model);
        var tile=Node(model,transform,at);
        var mesh=Instantiate(prefab,tile.transform,false);
        Renderer[] renderers=mesh.GetComponentsInChildren<Renderer>();
        Bounds bounds=renderers[0].bounds;
        foreach(var r in renderers) bounds.Encapsulate(r.bounds);
        float k=width/Mathf.Max(bounds.size.x,bounds.size.z);
        mesh.transform.localScale*=k;
        Vector3 center=tile.transform.InverseTransformPoint(bounds.center);
        mesh.transform.localPosition-=new Vector3(center.x,center.y-bounds.extents.y,center.z)*k;
        if(!mapped) {
            mapped=new Material(Shader.Find("Standard"));
            mapped.mainTexture=Resources.Load<Texture2D>("City/colormap");
            mapped.name="Цветовая карта Kenney";
        }
        foreach(var r in renderers) r.sharedMaterial=mapped;
        if (building) {
            // Используем геометрию здания, сохраняя свободную поверхность тротуара.
            foreach(var mf in mesh.GetComponentsInChildren<MeshFilter>()) {
                var collider=mf.gameObject.AddComponent<MeshCollider>(); collider.sharedMesh=mf.sharedMesh;
            }
        }
    }
    void BuildCity()
    {
        Sky(1.15f);
        Body("Земля",transform,new Vector3(0,-.25f,0),new Vector3(76,.5f,61),new Color(.36f,.48f,.33f));
        Body("Проспект",transform,new Vector3(0,.015f,0),new Vector3(6,.07f,43),Slate);
        for(int side=-1;side<=1;side+=2)
            Body("Тротуар",transform,new Vector3(side*4.25f,.09f,0),new Vector3(2.35f,.18f,42),Ivory);
        for(int z=-18;z<=18;z+=4) Body("Осевая разметка",transform,new Vector3(0,.067f,z),new Vector3(.11f,.012f,1.5f),Ivory,PrimitiveType.Cube,false);
        string[] houses={"building-small-a","building-small-b","building-small-c","building-small-d","building-garage"};
        for(int i=0;i<5;i++) {
            ImportTile(houses[i],new Vector3(-10,0,-16+i*8),7.8f,true);
            ImportTile(houses[(i+2)%5],new Vector3(10,0,-16+i*8),7.8f,true);
        }
        for(int z=-15;z<18;z+=8) ImportTile("grass-trees",new Vector3(-18,0,z),5.4f,true);
        Body("Платформа станции",transform,new Vector3(10,.28f,20.75f),new Vector3(18,.56f,2.5f),Ivory);
        for(int i=-1;i<=1;i+=2) Body("Рельс",transform,new Vector3(0,.1f,24+i*.68f),new Vector3(72,.16f,.10f),Slate);
        for(int i=-34;i<=34;i+=1) Body("Шпала",transform,new Vector3(i,.035f,24),new Vector3(.18f,.07f,1.9f),Timber);
    }
    RouteMover Route(string name,Vector3 start,Vector3 end,float sec,float pause,float offset=0)
    {
        var g=Node(name,transform,start);
        var rb=g.AddComponent<Rigidbody>(); rb.isKinematic=true; rb.interpolation=RigidbodyInterpolation.Interpolate;
        var route=g.AddComponent<RouteMover>();
        route.start=start;route.end=end;route.seconds=sec;route.pause=pause;route.offset=offset;
        return route;
    }
    void AddTraffic()
    {
        for(int i=0;i<2;i++) {
            var route=Route("Автомобиль "+(i+1),new Vector3(1.35f,0,-17),Vector3.zero,10,0,i*7.3f);
            route.roadLoop=true;
            var r=route.transform;
            Body("Кузов",r,new Vector3(0,.56f,0),new Vector3(1.35f,.52f,2.75f),i==0?Teal:Clay);
            Body("Кабина",r,new Vector3(0,.99f,-.18f),new Vector3(1.15f,.43f,1.38f),new Color(.23f,.35f,.4f));
            for(int a=-1;a<=1;a+=2) for(int b=-1;b<=1;b+=2)
                Body("Колесо",r,new Vector3(a*.67f,.30f,b*.86f),new Vector3(.52f,.10f,.52f),Slate,PrimitiveType.Cylinder).transform.localRotation=Quaternion.Euler(0,0,90);
        }
        for(int i=0;i<3;i++) {
            var route=Route("Пешеход "+i,new Vector3(-4.25f+i*.35f,.18f,-14),new Vector3(-4.25f+i*.35f,.18f,15),21+i,1,i*7);
            var r=route.transform;
            Body("Торс",r,new Vector3(0,.96f,0),new Vector3(.49f,.6f,.28f),i==0?Clay:Teal,PrimitiveType.Capsule);
            Body("Голова",r,new Vector3(0,1.66f,0),Vector3.one*.31f,Ivory,PrimitiveType.Sphere);
            route.leftLeg=Body("Левая нога",r,new Vector3(-.13f,.34f,0),new Vector3(.16f,.34f,.17f),Slate,PrimitiveType.Capsule).transform;
            route.rightLeg=Body("Правая нога",r,new Vector3(.13f,.34f,0),new Vector3(.16f,.34f,.17f),Slate,PrimitiveType.Capsule).transform;
        }
        var train=Route("Поезд у платформы",new Vector3(-24,.12f,24),new Vector3(11,.12f,24),13,5).transform;
        for(int car=0; car<2; car++) {
            float dx=car*6.5f;
            Body("Вагон",train,new Vector3(dx,1.2f,0),new Vector3(6.2f,1.65f,1.75f),Teal);
            for(int w=-2;w<=2;w++) for(int sign=-1;sign<=1;sign+=2)
                Body("Окно",train,new Vector3(dx+w,1.58f,sign*.88f),new Vector3(.75f,.53f,.025f),Ivory,PrimitiveType.Cube,false);
            for(int axle=-1;axle<=1;axle+=2) for(int side=-1;side<=1;side+=2)
                Body("Колесо",train,new Vector3(dx+axle*2,.28f,side*.68f),new Vector3(.48f,.09f,.48f),Slate,PrimitiveType.Cylinder).transform.localRotation=Quaternion.Euler(90,0,0);
        }
    }
}
