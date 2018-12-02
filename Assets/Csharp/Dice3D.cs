using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Dice3D : MonoBehaviour
{
    public Transform target;
    public Transform floor;
    public List<Transform> faces;

    Rigidbody _rigidbody;
    MeshRenderer _renderer;

    public Vector2 lastForces;
    public int value;

    public Action<int> onDiceStabilize;

    public void Initialize(Transform target, Transform floor)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<MeshRenderer>();

        this.target = target;
        this.floor = floor;

        transform.position = target.position;
    }

    public void Throw()
    {
        value = -1;
        float force = 250;
        float angle = UnityEngine.Random.Range(0, 360);

        float xForce = 250 + UnityEngine.Random.Range(-force, force) / 2;
        float yForce = 250 + UnityEngine.Random.Range(-force, force) / 2.5f;

        lastForces = new Vector2(UnityEngine.Random.value > 0.5 ? -xForce : xForce, UnityEngine.Random.value > 0.5 ? -yForce : yForce);

        _rigidbody.AddForce(lastForces, ForceMode.Force);
        _rigidbody.AddTorque(180 + UnityEngine.Random.value * 180, 180 + UnityEngine.Random.value * 180, 180 + UnityEngine.Random.value * 180);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Throw();
        }

        if (value == -1 && _rigidbody.IsSleeping())
        {
            value = -1;
            for (int i = 0; i < faces.Count; ++i)
            {
                Ray ray = new Ray(faces[i].position, faces[i].position - transform.position);

                RaycastHit[] hits = Physics.RaycastAll(ray);

                bool rethrow = false;
                foreach (var h in hits)
                {
                    if (h.transform == floor)
                    {
                        float distance = Vector3.Distance(faces[i].position, h.point);

                        if (distance > 0.1f)
                        {
                            rethrow = true;
                            break;
                        }

                        value = int.Parse(faces[i].name);
                        onDiceStabilize(value);
                    }
                }

                if (rethrow)
                {
                    Throw();
                    break;
                }
            }
        }
    }
}
