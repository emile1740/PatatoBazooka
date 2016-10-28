using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// BGM・SEを管理する
/// </summary>
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
	private const string BGM_PATH = "Sounds/BGM";
	private const string SE_PATH = "Sounds/SE";
	private const float DEF_VOL_SE = 1.0f;
	private const float DEF_VOL_BGM = 1.0f;
	private const float DEF_PITCH = 1.0f;

	public List<AudioClip> BGMList;
	public List<AudioClip> SEList;
	public int MaxSE = 10;
	public float BGM_Volume = 1.0f;
	public float SE_Volume = 1.0f;
	
	private AudioSource bgmSource = null;
	private AudioSource delayBgmSource = null;
	private List<AudioSource> seSources = null;
	private Dictionary<string,AudioClip> bgmDict = null;
	private Dictionary<string,AudioClip> seDict = null;

	/// <summary>
	/// 開始処理
	/// </summary>
	public void Awake ()
	{
		if (this != Instance) 
		{
			Destroy (this.gameObject);
			return;
		}

		DontDestroyOnLoad(this.gameObject);

		object[] bgmObj = Resources.LoadAll(BGM_PATH);
		object[] seObj = Resources.LoadAll(SE_PATH);
		Debug.Log(seObj.Length);

		//if (FindObjectsOfType (typeof(AudioListener)).All (o => !((AudioListener)o).enabled)) {
		//		this.gameObject.AddComponent<AudioListener> ();
		//}

		//AudioSourceを生成する
		//this.bgmSource = this.gameObject.AddComponent<AudioSource> ();
		this.seSources = new List<AudioSource> ();

		//AudioClipを生成する
		this.bgmDict = new Dictionary<string, AudioClip> ();
		this.seDict = new Dictionary<string, AudioClip> ();

		//Dictionary　添え字に名前を付けれる（日本語はダメ）
		//添え字部分のことをKeyと呼ぶ
		//seDict["itibanName"];


		//int[] a = new int[10];
		//for (int i=0; i<a.Length; i++) Debug.Log (a[i]);
		//foreach(int b in a) Debug.Log(b);
		//
		//foreachは配列の個数分繰り返す

		foreach (AudioClip bgm in bgmObj)
		{
			bgmDict[bgm.name] = bgm;
			BGMList.Add(bgm);
		}
		foreach (AudioClip se in seObj)
		{
			seDict[se.name] = se;
			SEList.Add(se);
		}

		BGM_Volume = DEF_VOL_BGM;
		SE_Volume = DEF_VOL_SE;
	}


	/// <summary>
	/// BGMを再生する
	/// </summary>
	/// <param name="bgmName">ファイル名</param>
	/// <param name="isLoop">ループさせるかどうか</param>
	//public void PlayBGM(string bgmName, bool isLoop = false, float volume = DEF_VOL_BGM, float pitch = DEF_PITCH, float delay = 0.0f)
	public void PlayBGM(string bgmName, bool isLoop, float volume, float pitch = DEF_PITCH, float delay = 0.0f)
	{
		if (!this.bgmDict.ContainsKey(bgmName))
			throw new ArgumentException(bgmName + " not found", "bgmName");
		if (this.bgmSource.clip == this.bgmDict[bgmName])
			return;

		this.bgmSource.Stop();
		this.bgmSource.clip = this.bgmDict[bgmName];
		this.bgmSource.loop = isLoop;
		this.bgmSource.pitch = pitch;
		this.bgmSource.volume = (volume == DEF_VOL_BGM) ? this.BGM_Volume : volume;

		Invoke("DelayPlayBGM", delay);
	}

    public int count;

	/// <summary>
	/// SEを設定する
	/// </summary>
	/// <param name="seName"></param>
	public void PlaySE(string seName, float volume = DEF_VOL_SE, float pitch = DEF_PITCH, float delay = 0.0f)
	{
		if (!this.seDict.ContainsKey(seName))
            throw new ArgumentException(seName + " not found", "seName");

        //プレイ中ではない最初の指定したSEを取得
		AudioSource source = this.seSources.FirstOrDefault(s => !s.isPlaying);

        if (source == null)
		{
			if (this.seSources.Count >= this.MaxSE)
			{
				Debug.Log("SE AudioSource is full");
				return;
			}
			source = this.gameObject.AddComponent<AudioSource>();
			this.seSources.Add(source);
		}

		source.clip = this.seDict[seName];
		source.volume = (volume == DEF_VOL_SE) ? this.SE_Volume : volume;
		source.pitch = pitch;

		source.Play();
    }

    /// <summary>
    /// SEを設定する
    /// </summary>
    /// <param name="seName"></param>
    public void repeatSE(string seName, float volume = DEF_VOL_SE) {
        //同じ名前のSEがあった場合は、そのSEが再生されていない時に再び再生する

        if (!this.seDict.ContainsKey(seName))
            throw new ArgumentException(seName + " not found", "seName");

        var sourceList = GetComponents<AudioSource>();
        AudioSource source = null;
        if(sourceList.Length > 0) {
            //コンポーネントの中のAudioSourceから、指定された名前のSEがあるかどうかを探す
            foreach (var s in sourceList) {
                if(s.clip.name == seName) {
                    source = s;
                    break;
                }
            }
        }

        //コンポーネントの中にAudioSourceがなかった場合、
        //もしくは指定したSEが再生中の場合は、今回のSEを追加
        if (sourceList.Length == 0 || source == null){

            if (this.seSources.Count >= this.MaxSE) {
                Debug.Log("SE AudioSource is full");
                return;
            }

            source = gameObject.AddComponent<AudioSource>();
            this.seSources.Add(source);
            source.clip = this.seDict[seName];
        }

        if (!source.isPlaying){
            source.volume = (volume == DEF_VOL_SE) ? this.SE_Volume : volume;
            source.Play();
        }        

    }



    /// <summary>
    /// BGMを停止する
    /// </summary>
    public void StopBGM()
	{
		this.bgmSource.Stop();
		this.bgmSource.clip = null;
	}

	/// <summary>
	/// SEを停止する
	/// </summary>
	public void StopSE ()
	{
		this.seSources.ForEach (s => s.Stop ());
	}

	/// <summary>
	/// 遅延させてBGMを再生する
	/// </summary>
	public void DelayPlayBGM()
	{
		this.bgmSource.Play();
	}

	/// <summary>
	/// 遅延させてSEを再生する
	/// </summary>
	public void DelayPlaySE()
	{
		
	}

    /// <summary>
    /// 指定したSEを停止する
    /// </summary>
    public void StopSE(string seName) {
        if (!this.seDict.ContainsKey(seName))
            throw new ArgumentException(seName + " not found", "seName");

        //プレイ中の指定したSEを取得
        AudioSource source = this.seSources.FirstOrDefault(s => s.isPlaying);
        if(source != null) source.Stop();
    }


    /// <summary>
    /// 指定したSEを再び再生できる場合は、その音を取得
    /// </summary>
    /// <param name="seName"></param>
    //public AudioSource isRePlaySE(string seName) {
    //    var sourceList = GetComponents<AudioSource>();

    //    foreach (var source in sourceList) {
    //        Debug.Log("sourceClipName " + source.clip.name);
    //    }

    //    //foreach (var source in sourceList) {
    //    //    if (source.clip.name == seName && !source.isPlaying) {
    //    //        return source;
    //    //    }
    //    //}
    //    return null;
    //}


}