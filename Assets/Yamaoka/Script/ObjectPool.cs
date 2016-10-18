﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : SingletonMonoBehaviour<ObjectPool> {

	//ゲームオブジェクトのDictionary
	private Dictionary<int, List<GameObject>> pooledGameObjects = new Dictionary<int, List<GameObject>>();

	//ゲームオブジェクトをpooledGameObjectsから取得する。必要であれば新たに作成する
	public GameObject GetGameObject(GameObject prefab, Vector3 position, Quaternion rotation){
		//プレハブのインスタンスIDをkeyとする
		int key = prefab.GetInstanceID ();

		//Dictionaryにkeyが存在しなければ作成する
		if (pooledGameObjects.ContainsKey (key) == false)
			pooledGameObjects.Add (key, new List<GameObject> ());

		List<GameObject> gameObjects = pooledGameObjects [key];

		GameObject go = null;

		for (int i = 0; i < gameObjects.Count; i++) {
			go = gameObjects[i];

			//現在非アクティブ（未使用）であれば
			if(go.activeInHierarchy == false){

				//位置を設定する
				go.transform.position = position;
                //Debug.Log(go.transform.position);
				//角度を設定する
				go.transform.rotation = rotation;

                //これから使用するのでアクティブにする
                go.SetActive(true);


				return go;
			}
		}

		//使用できるものがないので新たに生成する
		go = (GameObject)Instantiate (prefab, position, rotation);

		//ObjectPoolゲームオブジェクトの子要素にする
		go.transform.parent = transform;

		//リストに追加
		gameObjects.Add (go);

		return go;
	}

	//ゲームオブジェクトを非アクティブにする。こうすることで再利用可能状態にする
	public void ReleaseGameObject(GameObject go){
		//非アクティブにする
		go.SetActive (false);
	}
}
