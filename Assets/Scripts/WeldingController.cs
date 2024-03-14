using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeldingController : MonoBehaviour
{
    [SerializeField] private PowerManager powerManager;

    [SerializeField] private string m_WeldObjectTag;
    [SerializeField] private GameObject m_WeldingSpark;

    //[SerializeField] private float m_ElectrodeConsuptionTime = 5f;
   // [SerializeField] private float m_ElectrodeFinishScale = 0.2f;
   // [SerializeField] private GameObject m_BaseMesh;

    [SerializeField] private GameObject weldingEffectPrefab;

    [SerializeField] private AudioSource m_WeldingAudio;
   // [SerializeField] private AudioClip m_WeldingAudioFinish;
    [SerializeField] private ParticleSystem m_WeldingSparkPaticle;

    private bool isTouchingWeldObject = false;
    public float spawnWaitTime = 0.1f;

   // public UnityEvent onElectrodeConsumed;

    private List<GameObject> weldingEffects = new List<GameObject>();
    //private GameObject combinedObject;
    public float maxCombineInterval = 2.0f; // Maximum time interval between combines
    private float lastCombineTime;

    private void OnValidate()
    {
        m_WeldingSparkPaticle = m_WeldingSpark.GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        //m_WeldingAudio.Play();
    }

    private bool limitReached = false;

    public bool getLimitReached
    {
        get { return limitReached; }
    }
    private void Update()
    {
        if (!powerManager.canWeld) return;

        if (!isTouchingWeldObject) return;

        //if (!m_BaseMesh) return;
        //if (limitReached) return;

        //Vector3 currentScale = m_BaseMesh.transform.localScale;

        //currentScale.z = currentScale.z - (m_ElectrodeConsuptionTime * Time.deltaTime);

        //if (currentScale.z < m_ElectrodeFinishScale)
        //{
        //    //m_WeldingAudio.PlayOneShot(m_WeldingAudioFinish);
        //    limitReached = true;
        //    onElectrodeConsumed.Invoke();

        //    return;
        //}

        //m_BaseMesh.transform.localScale = currentScale;

        var emission = m_WeldingSparkPaticle.emission;

        emission.rateOverTimeMultiplier = (powerManager.weldVoltage * 1000) + 500;
    }

    private void CombineWeldingEffects(GameObject parent)
    {
        List<GameObject> previousWeldingEffects = new List<GameObject>(weldingEffects); // Create a copy

        weldingEffects.Clear(); // Clear the original list

        if (previousWeldingEffects.Count == 0)
            return;

        // Create a new combined object
        GameObject combinedObject = new GameObject("CombinedWeldingEffect");
        combinedObject.transform.position = Vector3.zero;
        combinedObject.transform.rotation = Quaternion.identity;
        combinedObject.transform.localScale = Vector3.one;

        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer combinedRenderer = combinedObject.AddComponent<MeshRenderer>();

        Material defaultMat = previousWeldingEffects[0].GetComponent<MeshRenderer>().sharedMaterial;

        // Create lists to store relevant components
        List<Mesh> meshList = new List<Mesh>();
        List<Transform> transformList = new List<Transform>();

        foreach (var weldingEffect in previousWeldingEffects)
        {
            MeshFilter meshFilter = weldingEffect.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshList.Add(meshFilter.sharedMesh);
                transformList.Add(weldingEffect.transform);
            }

            // Destroy the original object
            Destroy(weldingEffect);
        }

        // Combine the meshes
        CombineInstance[] combine = new CombineInstance[meshList.Count];
        for (int i = 0; i < meshList.Count; i++)
        {
            combine[i].mesh = meshList[i];
            combine[i].transform = transformList[i].localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine, true, true);

        // Set the combined mesh to the filter
        combinedMeshFilter.mesh = combinedMesh;

        // Copy the material from the first object (assumes all welding effects share the same material)
        if (meshList.Count > 0)
        {
            combinedRenderer.material = defaultMat;
        }

        combinedObject.transform.SetParent(parent.transform);
    }

    private bool instatiating = false;
    private IEnumerator InstantiateAndRotateWeldingEffect(Vector3 position, Quaternion rotation, Transform parentTransform)
    {

        instatiating = true;
        GameObject weldingEffect = Instantiate(weldingEffectPrefab, position, rotation, parentTransform);

        /*var rot = weldingEffect.transform.rotation;
        rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y + 90, rot.eulerAngles.z);
        weldingEffect.transform.rotation = rot;*/

        weldingEffects.Add(weldingEffect);

        yield return new WaitForSeconds(spawnWaitTime);

        instatiating = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected");
        if (!powerManager.canWeld)
            return;

        if (limitReached)
            return;

        if (!collision.gameObject.CompareTag(m_WeldObjectTag))
        {
            return;
        }
        Debug.Log(collision.gameObject.name);
        isTouchingWeldObject = true;

        if (!m_WeldingAudio.isPlaying)
        {
            m_WeldingAudio.Play();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!powerManager.canWeld) return;
        if (limitReached) return;

        if (!collision.gameObject.CompareTag(m_WeldObjectTag))
        {
            return;
        }

        m_WeldingSpark.gameObject.SetActive(true);

        if (Time.time - lastCombineTime > maxCombineInterval)
        {
            CombineWeldingEffects(collision.gameObject);
        }

        lastCombineTime = Time.time;

        //Instantiate(weldingEffectPrefab, collision.GetContact(0).point, Quaternion.identity, collision.gameObject.transform);

        Vector3 collisionPoint = collision.GetContact(0).point;
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        if (!instatiating) StartCoroutine(InstantiateAndRotateWeldingEffect(collisionPoint, rotation, collision.gameObject.transform));

    }

    private void OnCollisionExit(Collision collision)
    {
        if (!powerManager.canWeld) return;

        if (!collision.gameObject.CompareTag(m_WeldObjectTag))
        {
            return;
        }

        m_WeldingSpark.gameObject.SetActive(false);
        isTouchingWeldObject = false;

        //m_WeldingAudio.Pause();
        //m_WeldingAudio.enabled = false;

        if (m_WeldingAudio.isPlaying)
        {
            m_WeldingAudio.Pause();
        }
    }
}
