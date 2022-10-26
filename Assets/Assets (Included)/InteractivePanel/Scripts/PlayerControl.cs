/**************************************************************************/
/** 	© 2016 NULLcode Studio. License: CC 0.
/** 	Разработано специально для http://null-code.ru/
/** 	WebMoney: R209469863836, Z126797238132, E274925448496, U157628274347
/** 	Яндекс.Деньги: 410011769316504
/**************************************************************************/

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class PlayerControl : MonoBehaviour {

	[Header("General")]
	public float speed = 1.5f; // скорость движения
	public float acceleration = 100f; // ускорение
	public float stepTimer = 0.8f; // интервал шагов во время ходьбы (секунды)

	[Header("Head / Camera")]
	public Transform head;
	public float sensitivity = 5f; // чувствительность мыши
	public float headMinY = -40f; // ограничение угла для головы
	public float headMaxY = 40f;

	[Header("Jump")]
	public KeyCode jumpButton = KeyCode.Space; // клавиша для прыжка
	public float jumpForce = 10; // сила прыжка
	public float jumpDistance = 1.2f; // расстояние от центра объекта, до поверхности

	[Header("Run")]
	public KeyCode runButton = KeyCode.LeftShift; // клавиша для бега
	public float addForce = 5; // добавить к скорости

	private Vector3 direction;
	private int layerMask;
	private Rigidbody body;
	private float rotY, curSpeed;
	
	void Start () 
	{
		body = GetComponent<Rigidbody>();
		body.freezeRotation = true;
		layerMask = 1 << gameObject.layer | 1 << 2;
		layerMask = ~layerMask;
	}
	
	void FixedUpdate()
	{
		body.AddForce(direction.normalized * curSpeed * acceleration * body.mass);
		
		// Ограничение скорости, иначе объект будет постоянно ускоряться
		if(Mathf.Abs(body.velocity.x) > curSpeed)
		{
			body.velocity = new Vector3(Mathf.Sign(body.velocity.x) * curSpeed, body.velocity.y, body.velocity.z);
		}
		if(Mathf.Abs(body.velocity.z) > curSpeed)
		{
			body.velocity = new Vector3(body.velocity.x, body.velocity.y, Mathf.Sign(body.velocity.z) * curSpeed);
		}
	}

	bool GetJump() // проверяем, есть ли коллайдер под ногами
	{
		RaycastHit hit;
		Ray ray = new Ray(transform.position, Vector3.down);
		if(Physics.Raycast(ray, out hit, jumpDistance, layerMask))
		{
			return true;
		}

		return false;
	}

	void Rotation()
	{
		// управление головой (камерой)
		float rotX = head.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
		rotY += Input.GetAxis("Mouse Y") * sensitivity;
		rotY = Mathf.Clamp (rotY, headMinY, headMaxY);
		head.localEulerAngles = new Vector3(-rotY, rotX, 0);
	}

	void Update () 
	{
		curSpeed = speed;

		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Rotation();

		// вектор направления движения
		direction = new Vector3(h, 0, v);
		direction = head.TransformDirection(direction);
		direction = new Vector3(direction.x, 0, direction.z);

		if(Input.GetKey(runButton))
		{
			curSpeed = speed + addForce;
		}

		if(Input.GetKeyDown(jumpButton) && GetJump())
		{
			body.velocity = new Vector3(body.velocity.x, jumpForce, body.velocity.z);
		}
	}

	void OnDrawGizmosSelected() // для визуальной настройки jumpDistance, в режиме редактирования
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Vector3.down * jumpDistance);
	}
}
