using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TMPro.TextMeshProUGUI positive;
    public TMPro.TextMeshProUGUI negative;

    public float lifetime = 1f;

    public void Reset(Vector3 position, string value)
    {
        negative.text = value;
        positive.gameObject.SetActive(false);
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public void Reset(Vector3 position, int value)
    {
        if (value > 0)
        {
            positive.text = "+" + value;
            negative.gameObject.SetActive(false);
        }
        else
        {
            negative.text = value.ToString();
            positive.gameObject.SetActive(false);
        }

        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;

        if (positive.gameObject.activeInHierarchy)
        {
            transform.Translate(Vector2.up * Time.deltaTime * .5f);
        }
        else
        {
            transform.Translate(Vector2.down * Time.deltaTime * .5f);
        }


        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
