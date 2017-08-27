using UnityEngine;

public class InstantiateNoise : MonoBehaviour
{
    public GameObject noise;

    public float noiserate = 1f;

    private float rate = 0f;

    // Use this for initialization
    void Start()
    {
        rate = noiserate;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rate += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && rate >= noiserate)
        {
            rate = 0f;
            Vector3 mousePosOnFarPlane = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Vector3 mousePosOnNearPlane = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

            Vector3 mousePosToWorldPosOfFarPlane = Camera.main.ScreenToWorldPoint(mousePosOnFarPlane);
            Vector3 mousePosToWorldPosOfNearPlane = Camera.main.ScreenToWorldPoint(mousePosOnNearPlane);

            Ray ray = new Ray(mousePosToWorldPosOfNearPlane, mousePosToWorldPosOfFarPlane - mousePosToWorldPosOfNearPlane);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000f))
            {
                GameObject gm = Instantiate(noise, hitInfo.point, Quaternion.identity) as GameObject;
                Destroy(gm, 5f);
            }
        }
    }
}
