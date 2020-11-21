using System.Collections;
using UnityEngine;

public class LoadMap : MonoBehaviour
{
    // consts:
    private const float min_cam_height = 8;
    private const float max_cam_height = 18;
    private const float base_can_height = 15;
    private const float max_cam_shift = 8;
    private const float min_cam_shift = 3;
    private const float earth_rad = 6371000;
    public const string key = "BTXAvoGI5LkkTpGQ76JCFA0CwiHcg0Gr";


    public Camera camera;
    public GameObject sphere;
    private Vector2 sphere_position = new Vector2(45, 39);
    public Renderer map_render;
    private Vector2 map_center = new Vector2(45.06f, 38.94f); // latitude(вертикаль, широта), longitude(вбока, долгота)
    
    private string map_type = "hyb"; // map,hyb,sat,none,transparent
    private string url;
    
    private Texture2D [] textures = new Texture2D[7];
    private float [] map_distance_by_zoom = new float[7];
    private int zoom = 1;


    private bool is_mouse_down = false;
    private bool is_moveable = false;
    // Start is called before the first frame update
    void Start()
    {
        LoadAllMaps();
        FillMapDistance();
        sphere.transform.position = new Vector3(1000, 0, 1000);
        StartCoroutine(LocationTest());
    }

    private IEnumerator LocationTest()
    {
        Debug.Log("start LocationTest");
        // Сначала проверяем, включена ли у пользователя служба определения местоположения
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("location is not Enabled By User");
            yield break;
        }

        // Запускаем службу перед запросом местоположения
        Input.location.Start();

        // Ждем, пока служба инициализируется
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            --maxWait;
        }

        // Сервис не инициализировался через 20 секунд
        if (maxWait < 1)
        {
            Debug.Log("Истекло время ожидания");
            yield break;
        }

        // Соединение не удалось
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Невозможно определить местоположение устройства");
            yield break;
        }
        else
        {
            // Доступ предоставлен, и можно получить значение местоположения
            Debug.Log("Местоположение:" + Input.location.lastData.latitude + ";" + Input.location.lastData.longitude);
        }

        // Остановка службы, если нет необходимости постоянно запрашивать обновления местоположения
        Input.location.Stop();
    }


    // Update is called once per frame
    void Update()
    {
        if (!is_moveable)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Pressed secondary button.");
            ++zoom;
            zoom %= 7;
            if (textures[zoom] != null)
                map_render.material.mainTexture = textures[zoom];
        }
        var mw = -Input.GetAxis("Mouse ScrollWheel") * 2;
        if (camera.transform.position.y + mw < min_cam_height)
        {
            mw = 0;
            if (zoom!= 0)
			{
                --zoom;
                map_render.material.mainTexture = textures[zoom];
                camera.transform.position -= new Vector3(0, camera.transform.position.y - max_cam_height, 0);
                MoveSphere();
            }
        }
        if (camera.transform.position.y + mw > max_cam_height)
        {
            mw = 0;
            if (zoom != 6)
            {
                ++zoom;
                map_render.material.mainTexture = textures[zoom];
                camera.transform.position -= new Vector3(0, camera.transform.position.y - min_cam_height, 0);
                MoveSphere();
            }
        }
        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");
        if (!is_mouse_down)
        {
            x = 0;
            y = 0;
        }
        camera.transform.position += new Vector3(x, mw, y);
        if (MustWeUpdateMap())
            ResetMapCenter();

    }

    private void OnMouseDown()
    {
        if (is_moveable)
            is_mouse_down = true;
    }

    private void OnMouseUp()
    {
        is_mouse_down = false;
    }

    private void LoadAllMaps()
    {
        is_moveable = false;
        is_mouse_down = false;
        for (int i = 0; i < 7; ++i)
            StartLoadMap(map_center, 17 - i * 2, i, i == zoom);

        //Debug.Log("map_center: " + map_center.x + "; " + map_center.y);

    }

    private void StartLoadMap(Vector2 position, int zoom, int texture_index, bool activate = false)
    {
        url = "http://open.mapquestapi.com/staticmap/v4/getmap?key="
    + key
    + "&size=1280,1280&zoom=" + zoom
    + "&type=" + map_type
    + "&center=" + position.x.ToString().Replace(",", ".") + "," + position.y.ToString().Replace(",", ".");
        //Debug.Log(url);
        StartCoroutine(LoadImage(texture_index, activate));
    }

    private IEnumerator LoadImage(int texture_index, bool activate = false)
    {
        var www = new WWW(url);
        while (!www.isDone)
        {
            //Debug.Log(www.progress);
            yield return new WaitForSeconds(0.1f); ;
        }

        if (www.error == null)
        {
            //Debug.Log("Updating map 100%");
            //Debug.Log("map ready");
            yield return new WaitForSeconds(0.5f);
            textures[texture_index] = new Texture2D(1280, 1280, TextureFormat.RGB24, false);
            

            www.LoadImageIntoTexture(textures[texture_index]);
            if (activate)
            {
                map_render.material.mainTexture = textures[texture_index];
                is_moveable = true;
                camera.transform.position = new Vector3(0, camera.transform.position.y, 0);
                MoveSphere();
            }
        }
		else
		{
            Debug.Log(www.error);
            yield return new WaitForSeconds(0.5f);
            map_render.material.mainTexture = null;
        }

        map_render.enabled = true;
    }

    private bool MustWeUpdateMap()
	{
        float x = Mathf.Abs(camera.transform.position.x);
        float z = Mathf.Abs(camera.transform.position.z);

        float max_shift = max_cam_shift - (camera.transform.position.y - min_cam_height)
            * (max_cam_shift - min_cam_shift) / (max_cam_height - min_cam_height);
        return x > max_shift || z > max_shift;
    }

    private void ResetMapCenter()
	{
        map_center.x -= MettersToLatitude(map_distance_by_zoom[zoom] * camera.transform.position.z);
        map_center.y -= MettersToLongitude(map_distance_by_zoom[zoom] * camera.transform.position.x);
        LoadAllMaps();
    }

    private float LatitudeToMetters(float lat)
	{
        return lat * 111000;
	}

    // https://leonid.shevtsov.me/post/chto-ty-dolzhen-znat-pro-geograficheskie-koordinaty/
    private float LongitudeToMetters(float lon)
    {
        return lon * earth_rad * (Mathf.PI / 180) * Mathf.Cos(map_center.x * Mathf.PI / 180);
        //return lon * earth_rad * Mathf.Cos(map_center.x * Mathf.PI / 180);
    }

    private float MettersToLatitude(float met)
    {
        return met / 111000;
    }

    private float MettersToLongitude(float met)
    {
        return met / (earth_rad * (Mathf.PI / 180) * Mathf.Cos(map_center.x * Mathf.PI / 180));
        //return met / earth_rad * Mathf.Cos(map_center.x * Mathf.PI / 180);
    }

    private void FillMapDistance()
    {
        map_distance_by_zoom[0] = 100f / 2.75f;
        map_distance_by_zoom[1] = 200f / 1.45f;
        map_distance_by_zoom[2] = 1000f / 1.7f;
        map_distance_by_zoom[3] = 5000f / 2.2f;
        map_distance_by_zoom[4] = 20000f / 2.2f;
        map_distance_by_zoom[5] = 100000f / 2.75f;
        map_distance_by_zoom[6] = 200000f / 1.45f;
    }

    private void MoveSphere()
	{
        var lat = -LatitudeToMetters(sphere_position.x - map_center.x);
        var lon = -LongitudeToMetters(sphere_position.y - map_center.y);
        //Debug.Log("lat: " + lat + "; lon: " + lon + "; zoom: " + zoom);
        var vector_to_sphere = new Vector2(lat / map_distance_by_zoom[zoom], lon / map_distance_by_zoom[zoom]);
        //Debug.Log("vector_to_sphere 2: " + vector_to_sphere.x + "; " + vector_to_sphere.y);
        sphere.transform.position = new Vector3(lon / map_distance_by_zoom[zoom], 0, lat / map_distance_by_zoom[zoom]);
        //Debug.Log("sphere.transform.position: " + sphere.transform.position);
    }
}
