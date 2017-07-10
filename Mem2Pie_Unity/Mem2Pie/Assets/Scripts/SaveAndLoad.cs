using UnityEngine;
using System.Collections;
using System.Collections.Generic;   
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public static class SaveAndLoad
{

	public static void Save()
	{
		var binaryFormatter     = new BinaryFormatter();
		var memoryStream        = new MemoryStream();

		// score를 바이트 배열로 변환해서 저장합니다.
		binaryFormatter.Serialize(memoryStream, InputMemController.memList);

		// 그것을 다시 한번 문자열 값으로 변환해서 
		// 'HighScore'라는 스트링 키값으로 PlayerPrefs에 저장합니다.
		PlayerPrefs.SetString(Gallerytest.myfilePath, Convert.ToBase64String(memoryStream.GetBuffer()));
	}

	public static  void Load()
	{
		// 'HighScore' 스트링 키값으로 데이터를 가져옵니다.
		var data = PlayerPrefs.GetString(Gallerytest.myfilePath);

		if (!string.IsNullOrEmpty (data)) {
			var binaryFormatter = new BinaryFormatter ();
			var memoryStream = new MemoryStream (Convert.FromBase64String (data));

			// 가져온 데이터를 바이트 배열로 변환하고
			// 사용하기 위해 다시 리스트로 캐스팅해줍니다.
			InputMemController.memList = (ArrayList)binaryFormatter.Deserialize (memoryStream);
		} else {
			InputMemController.memList = new ArrayList();
		}
	}
}
