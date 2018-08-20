using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
  [RequireComponent(typeof(Camera))]
  class CameraController : MonoBehaviour
  {
    public int speed = 5;
    public int rotationSpeed = 5;
    public bool invertY = false;
    public bool invertX = false;
    private Vector3 angles;

    void Start()
    {
      Cursor.visible = false;
      angles = transform.localEulerAngles;
    }

    void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        Cursor.visible = true;
      } else if (Input.GetMouseButtonDown(0))
      {
        Cursor.visible = false;
      }

      float horizontalSpeed = Input.GetAxis("Horizontal") * speed;
      float verticalSpeed = Input.GetAxis("Vertical") * speed;
      float turnX = Input.GetAxis("Mouse Y") * (invertY ? 1 : -1);
      float turnY = Input.GetAxis("Mouse X") * (invertX ? -1 : 1);

      angles.x = (angles.x + turnX) % 360;
      angles.y = (angles.y + turnY) % 360;
      angles.z = 0;
      transform.localEulerAngles = angles;

      transform.position = transform.position + transform.forward * verticalSpeed + transform.right * horizontalSpeed;


    }
  }
}
