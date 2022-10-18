/**************************************************************************/
/** 	© 2016 NULLcode Studio. License: CC 0.
/** 	Разработано специально для http://null-code.ru/
/** 	WebMoney: R209469863836, Z126797238132, E274925448496, U157628274347
/** 	Яндекс.Деньги: 410011769316504
/**************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]

public class CalculateCanvasScale : MonoBehaviour {

	[Header("Ширина в метрах:")]
	[SerializeField] private float widthInMeters = 1;

	public void Calculate()
	{
		Canvas target = GetComponent<Canvas>();

		if(target.renderMode != RenderMode.WorldSpace)
		{
			Debug.Log(this + " выбран неверный режим 'Render Mode' необходимо установить его в 'World Space'");
			return;
		}

		RectTransform tr = GetComponent<RectTransform>();
		float size = widthInMeters/tr.sizeDelta.x;
		tr.localScale = new Vector3(size, size, size);

		BoxCollider coll = GetComponent<BoxCollider>();

		if(coll == null)
		{
			BoxCollider box = gameObject.AddComponent<BoxCollider>();
			box.size = new Vector3(tr.sizeDelta.x, tr.sizeDelta.y, 1);
		}
		else
		{
			coll.size = new Vector3(tr.sizeDelta.x, tr.sizeDelta.y, 1);
		}
	}
}
