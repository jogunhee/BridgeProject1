﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Handler : MonoBehaviour
{

	public static bool isPause;
	public static bool isTuto;

    // Use this for initialization
    private ArrayList itemList;
    private int numOfItem;
    //private usingitem drillItem,cushionItem,wallItem,jumpingItem;
    private int numDie = 0;
    private int score = 0;
    private float time = 60;
    private bool isBtnOn = false;
    private bool isMoved = false;
    private GameObject tpObj = null;
    private Vector3 tpPos;
    private Vector2 initMousePos;
    //private int[] seqOfItem = new int[userData.numOfItemKind];
    private int OnItem;

    //Camera Opt
    public float minY = 50, maxY = 50, minX = 50, maxX = 50;
    //Map Obj
    public GameObject spawn;
    public GameObject WarriorSpawn;
    public GameObject background;
    //Moving Obj
    public GameObject Slime;
    //Item Obj
    public GameObject upDrill;
    public GameObject Cusion;
    public GameObject Wall;
    //Dummy Obj
    public GameObject dummyCusion;
    public GameObject dummyWall;
    public GameObject dummyjumping;
    //UI Obj;
    public Text itemNumText;
    public GameObject[] icon = new GameObject[userData.numOfItemKind];
    public Text scoreText;
    public Text timeText;
    public Text endScoreText;
    public Text ClearText;
	public Text stageNumText;
    public Image[] scoreStar;
    public Sprite noStar;
    public GameObject clearPnl;
    public Button nextBtn;
    //etc.
    //public SpriteRenderer slimeSpawnSr;
    //public Sprite[] slimeSpawnSp;
    public GameObject slimes;
    public AudioClip winSE;
    public AudioClip loseSE;
    IEnumerator showClearPanel(){
        yield return new WaitForSeconds(2.0f);
        if(!isTuto && score < 3){
             AudioSource.PlayClipAtPoint(winSE,new Vector2(0,0));
        }else{
             AudioSource.PlayClipAtPoint(winSE,new Vector2(0,0));
        }
        clearPnl.SetActive(true);
        Time.timeScale = 0;
    }
    void checkClear()
    {
		if (isTuto && score > 0) {
			PlayerPrefs.SetInt ("isNotFirst", 1);
			endScoreText.text = "X 1/1";
			nextBtn.interactable = true;
            AudioSource.PlayClipAtPoint(winSE,new Vector2(0,0));
            StartCoroutine(showClearPanel());
			PlayerPrefs.SetInt ("Stage0Score", 20);
			if(PlayerPrefs.GetInt("clearedStage") == -1)
				PlayerPrefs.SetInt("clearedStage",SceneManager.GetActiveScene().buildIndex-3);
			isTuto = false;
		}
        else if (score + numDie >= 20)
        {
			int curStageNum = SceneManager.GetActiveScene ().buildIndex - 3;
			string stageText = "Stage" + curStageNum + "Score";
			if (score > PlayerPrefs.GetInt(stageText)) //userData.stage[userData.curStageNum].getScore())
            {
                // Debug.Log("setsore1:"+userData.stage[userData.curStageNum].getScore());
				PlayerPrefs.SetInt(stageText,score);
                //userData.stage[userData.curStageNum].setScore(score);
                //Debug.Log("setsore2:" + userData.stage[userData.curStageNum].getScore());
            }
			if (userData.curStageNum > PlayerPrefs.GetInt("clearedStage") && numDie < 18)
            {
				PlayerPrefs.SetInt("clearedStage",SceneManager.GetActiveScene().buildIndex-3);
            }
			if (userData.curStageNum <= PlayerPrefs.GetInt("clearedStage"))
                nextBtn.interactable = true;
            
            endScoreText.text = "X " + score + "/20";
            if (score < 3)
            {
                ClearText.text = "FAIL!";
                scoreStar[0].sprite = noStar;
                scoreStar[1].sprite = noStar;
                scoreStar[2].sprite = noStar;
            }
            else if (score < 10)
            {
                scoreStar[1].sprite = noStar;
                scoreStar[2].sprite = noStar;
            }
            else if (score >= 10 && score < 20){
                scoreStar[2].sprite = noStar;
            }
			stageNumText.text = "Stage" + (++curStageNum);
            StartCoroutine(showClearPanel());
        }
    }
    void createDummy(GameObject obj)
    {
        numOfItem--;
        UpdateItemNum();
        isBtnOn = true;
        tpPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tpObj = (GameObject)Instantiate(obj, new Vector2(tpPos.x, tpPos.y), transform.rotation);
        tpObj.transform.SetParent(slimes.transform);
        tpObj.SetActive(true);
        initItemPosition();
    }
    public void initIcon()
    {
        foreach (GameObject key in icon)
        {
            key.GetComponent<SpriteRenderer>().color = new Color32(255,255,255,255);//transform.localScale = new Vector2(1.0f, 1.0f);
        }
        OnItem = -1;
    }
    void selectedItem(int n)
    {
        initIcon();
        //icon[n].transform.localScale = new Vector2(1.15f, 1.15f);
        icon[n].GetComponent<SpriteRenderer>().color = new Color32(94,70,70,255);
        OnItem = n;
    }
    void checkTouch()
    {
		if (Input.GetMouseButtonDown(0) == true && Time.timeScale > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if (hit)
            {
                string tmp = hit.transform.gameObject.name;
                Debug.Log(tmp);
                if (numOfItem > 0)
                {
					if (isTuto) {
						//튜토리얼 일때  
						switch (tmp) {
						case "DrillIcon":
							if (Tutorial.isDrillCo) {
								createDummy (upDrill);
								selectedItem (0);
							}
							break;
						case "CushionIcon":
							if (Tutorial.isCushionCo) {
								createDummy (dummyCusion);
								selectedItem (1);
								//Tutorial.isCushionCo = false;
							}
							break;
						case "WallIcon":
							if (Tutorial.isWallCo) {
								createDummy (dummyWall);
								selectedItem (2);
							}
							break;
						case "JumpingIcon":
							if (Tutorial.isJumpingCo) {
								createDummy (dummyjumping);
								selectedItem (3);
							}
							break;
						//튜토리얼 일때 물리형 클릭소환
						case "CushionCheck":
							if (Tutorial.isCushionCo) {
								Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
								Instantiate (Cusion, new Vector2 (temp.x, temp.y), transform.rotation);
								numOfItem--;
								UpdateItemNum ();
								Tutorial.isCushionCo = false; 
								initIcon ();
							}
							break;
						case "WallCheck":
							if (Tutorial.isWallCo) {
								Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
								Instantiate (Wall, new Vector2 (temp.x, temp.y), transform.rotation);
								numOfItem--;
								UpdateItemNum ();
								Tutorial.isWallCo = false;
								initIcon ();
							}
							break;
						default:
							//설치형클릭후 땅 클릭했을때
							if (OnItem == 0) {
								createDummy (upDrill);
								tpObj.GetComponent<BoxCollider2D> ().enabled = true;
							} else if (OnItem == 3) {
								createDummy (dummyjumping);
								tpObj.GetComponent<BoxCollider2D> ().enabled = true;
							}
							if(!Tutorial.isCushionCo)
								initIcon ();
							if (Time.timeScale != 0) {
								initMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
								isMoved = true;
							}

							break;
						}
					} else {
						switch (tmp) {
						case "DrillIcon":
							createDummy (upDrill);
							selectedItem (0);
							break;
						case "CushionIcon":
							createDummy (dummyCusion);
							selectedItem (1);
							break;
						case "WallIcon":
							createDummy (dummyWall);
							selectedItem (2);
							break;
						case "JumpingIcon":
							createDummy (dummyjumping);
							selectedItem (3);
							break;
						default:
                            //설치형클릭후 땅 클릭했을때
							if (OnItem == 0) {
								createDummy (upDrill);
								Tutorial.isDrillCo = false;
								//tpObj.GetComponent<BoxCollider2D> ().enabled = true;
							} else if (OnItem == 3) {
								createDummy (dummyjumping);
								Tutorial.isJumpingCo = false;
								//tpObj.GetComponent<BoxCollider2D> ().enabled = true;
							}
							initIcon ();
							if (Time.timeScale != 0) {
								initMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
								isMoved = true;
							}
							break;
						}
					}
                }

            }
			else if (OnItem > -1 && !isTuto)
            {//물리 적용된것
                tpPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                switch (OnItem)
                {
				case 1:
					Instantiate (Cusion, new Vector2 (tpPos.x, tpPos.y), transform.rotation);
					numOfItem--;
					UpdateItemNum ();
					break;
				case 2:
					Instantiate (Wall, new Vector2 (tpPos.x, tpPos.y), transform.rotation);
					numOfItem--;
					UpdateItemNum ();
					break;
                }
                initIcon();

            }
            else
            {
                if (Time.timeScale != 0)
                {
                    initMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    isMoved = true;
                }
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            //마우스 뗌.
            if (isBtnOn)
            {
                isBtnOn = false;
                string temp = tpObj.gameObject.tag;
				switch (temp) {
				case "drill":
					tpObj.GetComponent<BoxCollider2D> ().enabled = true;
					break;
				case "cushion":
						
					tpObj.SetActive (false);
					tpObj = Instantiate (Cusion, new Vector2 (tpPos.x, tpPos.y), transform.rotation) as GameObject;
					if (tpObj.transform.position.y < -3) {
						Destroy (tpObj);
						canceledItem ();
					} else {
						//Tutorial.isCushionCo = false;
						if(!isTuto)
							initIcon ();
					}
					break;
				case "Wall":
					tpObj.SetActive (false);
					tpObj = Instantiate (Wall, new Vector2 (tpPos.x, tpPos.y), transform.rotation) as GameObject;
					if (tpObj.transform.position.y < -3) {
						Destroy (tpObj);
						canceledItem ();
					} else {
						//Tutorial.isWallCo = false;
						if(!isTuto)
							initIcon ();
					}
					break;
				case "Jumping":
					tpObj.GetComponent<BoxCollider2D> ().enabled = true;
					break;
				default:
					isMoved = false;
					break;
				}

            }
        }
        else if (Input.GetMouseButton(0))
        {
			if (isBtnOn)
            {
                tpPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                tpObj.transform.position = new Vector2(tpPos.x, tpPos.y); //마우스 누르고 있음.
            }
            else
            {
                if (Time.timeScale != 0 && isMoved)
                {
                    Vector2 worldpoint;
                    worldpoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 diffPos = (worldpoint - initMousePos) * Time.deltaTime * 20;

                    transform.position = new Vector3(Mathf.Clamp((transform.position.x - diffPos.x), minX, maxX), Mathf.Clamp((transform.position.y - diffPos.y), minY, maxY), -23);
                    initMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }

            }

        }
        /*
		 if(0 < Input.touchCount) 
		   {
		    for(int t = 0; t < 1; ++t)  //멀티 터치를 막아놨음. t < 1 의 뒷부분의 숫자가 최대 멀티터치 숫자임.
		    {
		     if(Input.GetTouch(t).phase == TouchPhase.Began)
		     {
		      //터치 시작.
		     }
		     else if(Input.GetTouch(t).phase == TouchPhase.Ended)
		     {
		      //터치 끝.
		     }
		     else if(Input.GetTouch(t).phase == TouchPhase.Moved || Input.GetTouch(t).phase == TouchPhase.Stationary) 
		     {
		//터치 중, 터치후 이동중.
		     }
		    }
		   }
		 * /
		/*for (int i = 0; i < Input.touchCount; ++i) {  //touch check
			if (Input.GetTouch (i).phase == TouchPhase.Began && isdrillOn) {
				Instantiate (upDrill, Input.GetTouch (i).position, transform.rotation);
				RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (i).position), Vector2.zero);
				if (hit) {
					isdrillOn = false;
					Destroy (hit.collider.gameObject);
				}
			} else if (Input.GetTouch (i).phase == TouchPhase.Moved && isdrillOn) {
				upDrill.transform.position = Input.GetTouch (i).position;
			} else if (Input.GetTouch (i).phase == TouchPhase.Ended && isdrillOn) {
				RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (i).position), Vector2.zero);
				if (hit) {
					isdrillOn = false;
					Destroy (hit.collider.gameObject);
				}
			}
		}*/
    }
    void checkEndGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { //Quit game
            Application.Quit();
        }
    }
    void checkTime()
    {
		if (!isPause) {
			time -= Time.deltaTime;
			timeText.text = (int)time / 60 + ":";
			if ((int)time % 60 > 9) {
				timeText.text += (int)time % 60;
			} else {
				timeText.text += "0" + (int)time % 60;
			}
			if (time <= 0) {
				score = 0;
				numDie = 20;
			}
		}
    }
    public void addDieCnt()
    {
        numDie++;
    }
    public void addScore()
    {
        score++;
        scoreText.text = score.ToString();
    }
    public void UpdateItemNum()
    { //남은 아이템 갯수
        itemNumText.text = numOfItem.ToString();
    }
    public void initItemPosition()
    {
        float x, y, posY = -0.23f;
        foreach (GameObject key in icon)
        {
            x = key.transform.localPosition.x;
            y = key.transform.localPosition.y;
            if (y >= -0.23f)
                key.transform.localPosition = new Vector2(x, posY);
        }
    }
    public void canceledItem()
    {
        numOfItem++;
        UpdateItemNum();
    }
    //시작처리

    void setItem()
    {
        float x = -2.85f, y = -0.20f;
        itemList = userData.stage[userData.curStageNum].getItemList();
        numOfItem = userData.stage[userData.curStageNum].getNumOfItem();
        foreach (usingitem usi in itemList)
        {
            string str = usi.getItemName();
            switch (str)
            {
                case "drill":
                    icon[0].transform.localPosition = new Vector2(x, y);
                    //seqOfItem [0] = i;
                    //drillItem = usi.clone ();
                    break;
                case "cushion":
                    icon[1].transform.localPosition = new Vector2(x, y);
                    //seqOfItem [1] = i;
                    //cushionItem = usi.clone ();
                    break;
                case "wall":
                    icon[2].transform.localPosition = new Vector2(x, y);
                    //seqOfItem [2] = i;
                    //wallItem = usi.clone ();
                    break;
                case "jumping":
                    icon[3].transform.localPosition = new Vector2(x, y);
                    //seqOfItem [3] = i;
                    //jumpingItem = usi.clone ();
                    break;
            }
            x += 2.33f;
        }
        UpdateItemNum();
    }

   

    void Start()
    {
		Screen.SetResolution(Screen.width,(Screen.width/16)*9,true);
        Time.timeScale = 1;
		userData.curStageNum = SceneManager.GetActiveScene ().buildIndex - 3;
        setItem();
        background = GameObject.Find("Background");
		
		OnItem = -1;
        slimes = GameObject.Find("MovingObj");
        //WarriorSpawnSr = WarriorSpawn.GetComponent<SpriteRenderer> ();
        //slimeSpawnSr = spawn.GetComponent<SpriteRenderer> ();
    }
    // Update is called once per frame
    void Update()
    {
        checkTime();
        checkEndGame();
        checkTouch();
        checkClear();
    }

}
