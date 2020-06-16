using System.Collections;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    float fall = 0;
    private float fallSpeed = 1.0f;
    public bool alowRotation = true;
    public bool limitRotation = false;

    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;
    public AudioClip lineClearedSound;

    private float continuousVerticalSpeed = 0.05f;
    private float continuousHorizontalSpeed = 0.1f;
    private float buttonDownWaitMax = 0.2f;

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    //private float buttonDownWaitTimer = 0;
    private float buttonDownWaitTimeVertical = 0;
    private float buttonDownWaitTimeHorizontal = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    private AudioSource audioSource;

    public int individualScore = 100;
    private float individualScoreTime = 0;

    private int touchSensitivityHorizontal = 8;
    private int touchSensitivityVertical = 4;
    Vector2 previousUnitPosition = Vector2.zero;
    Vector2 direction = Vector2.zero;
    bool moved = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //fallSpeed = GameObject.Find("Grid").GetComponent<Game>().fallSpeed;
        fallSpeed = Game.fallSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();
        UpdateIndividualScore();
    }

    void UpdateIndividualScore()
    {
        if (individualScoreTime < 1)
        {
            individualScoreTime += Time.deltaTime;
        }
        else
        {
            individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }
#if UNITY_ANDROID
    void CheckUserInput ()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                previousUnitPosition = new Vector2(t.position.x, t.position.y);
            }
            else if (t.phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = t.deltaPosition;
                direction = touchDeltaPosition.normalized;

                if (Mathf.Abs(t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal && direction.x < 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10)
                {
                    //MoveLeft
                    transform.position += new Vector3(-1, 0, 0);
                    if (CheckIsValidPosition())
                    {
                        FindObjectOfType<Game>().UpdateGrid(this);
                        PlayMoveAudio();
                    }
                    else
                    {
                        transform.position += new Vector3(1, 0, 0);
                    }
                    previousUnitPosition = t.position;
                    moved = true;
                }
                else if (Mathf.Abs(t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal && direction.x > 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10)
                {
                    //MoveRight
                    transform.position += new Vector3(1, 0, 0);
                    if (CheckIsValidPosition())
                    {
                        FindObjectOfType<Game>().UpdateGrid(this);
                        PlayMoveAudio();
                    }
                    else
                    {
                        transform.position += new Vector3(-1, 0, 0);
                    }
                    previousUnitPosition = t.position;
                    moved = true;
                }
                else if (Mathf.Abs(t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal && direction.y < 0 && t.deltaPosition.x > -10 && t.deltaPosition.x < 10)
                {
                    transform.position += new Vector3(0, -1, 0);
                    //fall = Time.time;
                    if (CheckIsValidPosition())
                    {
                        FindObjectOfType<Game>().UpdateGrid(this);
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            PlayMoveAudio();
                        }
                    }
                    else
                    {
                        transform.position += new Vector3(0, 1, 0);
                        PlayLandAudio();
                        bool check = FindObjectOfType<Game>().DeleteRow();
                        if (check) PlayLineClearedAudio();
                        Game.currentScore += individualScore >> 3;

                        if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
                        {
                            FindObjectOfType<Game>().GameOver();
                        }

                        enabled = false;
                        FindObjectOfType<Game>().SpawnNextTetromino();
                    }
                    fall = Time.time;
                    previousUnitPosition = t.position;
                    moved = true;
                }
            } else if (t.phase == TouchPhase.Ended)
            {
                if (!moved && t.position.x > Screen.width / 4)
                {
                    if (alowRotation)
                    {
                        if (limitRotation)
                        {
                            if (transform.rotation.eulerAngles.z >= 90)
                            {
                                transform.Rotate(0, 0, -90);
                            }
                            else
                            {
                                transform.Rotate(0, 0, 90);
                            }
                        }
                        else
                        {
                            transform.Rotate(0, 0, 90);
                        }
                        if (CheckIsValidPosition())
                        {
                            FindObjectOfType<Game>().UpdateGrid(this);
                            PlayRotateAudio();
                        }
                        else
                        {
                            if (limitRotation)
                            {
                                if (transform.rotation.eulerAngles.z >= 90)
                                {
                                    transform.Rotate(0, 0, -90);
                                }
                                else
                                {
                                    transform.Rotate(0, 0, 90);
                                }
                            }
                            else
                            {
                                transform.Rotate(0, 0, -90);
                            }

                        }
                    }
                }
                moved = false;
            }
        }
    }
#endif
    //void CheckUserInput()
    //{
    //    if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
    //    {
    //        movedImmediateHorizontal = false;
    //        horizontalTimer = 0;
    //        buttonDownWaitTimeHorizontal = 0;
    //    }

    //    if (Input.GetKeyUp(KeyCode.DownArrow))
    //    {
    //        movedImmediateVertical = false;
    //        buttonDownWaitTimeVertical = 0;
    //        verticalTimer = 0;
    //    }

    //    if (Input.GetKey(KeyCode.RightArrow))
    //    {
    //        if (movedImmediateHorizontal)
    //        {
    //            if (buttonDownWaitTimeHorizontal < buttonDownWaitMax)
    //            {
    //                buttonDownWaitTimeHorizontal += Time.deltaTime;
    //                return;
    //            }
    //            if (horizontalTimer < continuousHorizontalSpeed)
    //            {
    //                horizontalTimer += Time.deltaTime;
    //                return;
    //            }
    //        }
    //        if (!movedImmediateHorizontal) movedImmediateHorizontal = true;
    //        horizontalTimer = 0;

    //        transform.position += new Vector3(1, 0, 0);
    //        if (CheckIsValidPosition())
    //        {
    //            FindObjectOfType<Game>().UpdateGrid(this);
    //            PlayMoveAudio();
    //        }
    //        else
    //        {
    //            transform.position += new Vector3(-1, 0, 0);
    //        }
    //    }
    //    if (Input.GetKey(KeyCode.LeftArrow))
    //    {
    //        if (movedImmediateHorizontal)
    //        {
    //            if (buttonDownWaitTimeHorizontal < buttonDownWaitMax)
    //            {
    //                buttonDownWaitTimeHorizontal += Time.deltaTime;
    //                return;
    //            }
    //            if (horizontalTimer < continuousHorizontalSpeed)
    //            {
    //                horizontalTimer += Time.deltaTime;
    //                return;
    //            }
    //        }
    //        if (!movedImmediateHorizontal) movedImmediateHorizontal = true;
    //        horizontalTimer = 0;

    //        transform.position += new Vector3(-1, 0, 0);
    //        if (CheckIsValidPosition())
    //        {
    //            FindObjectOfType<Game>().UpdateGrid(this);
    //            PlayMoveAudio();
    //        }
    //        else
    //        {
    //            transform.position += new Vector3(1, 0, 0);
    //        }
    //    }
    //    if (Input.GetKeyDown(KeyCode.UpArrow))
    //    {
    //        if (alowRotation)
    //        {
    //            if (limitRotation)
    //            {
    //                if (transform.rotation.eulerAngles.z >= 90)
    //                {
    //                    transform.Rotate(0, 0, -90);
    //                }
    //                else
    //                {
    //                    transform.Rotate(0, 0, 90);
    //                }
    //            }
    //            else
    //            {
    //                transform.Rotate(0, 0, 90);
    //            }
    //            if (CheckIsValidPosition())
    //            {
    //                FindObjectOfType<Game>().UpdateGrid(this);
    //                PlayRotateAudio();
    //            }
    //            else
    //            {
    //                if (limitRotation)
    //                {
    //                    if (transform.rotation.eulerAngles.z >= 90)
    //                    {
    //                        transform.Rotate(0, 0, -90);
    //                    }
    //                    else
    //                    {
    //                        transform.Rotate(0, 0, 90);
    //                    }
    //                }
    //                else
    //                {
    //                    transform.Rotate(0, 0, -90);
    //                }

    //            }
    //        }

    //    }
    //    if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
    //    {
    //        if (movedImmediateVertical)
    //        {
    //            if (buttonDownWaitTimeVertical < buttonDownWaitMax)
    //            {
    //                buttonDownWaitTimeVertical += Time.deltaTime;
    //                return;
    //            }
    //            if (verticalTimer < continuousVerticalSpeed)
    //            {
    //                verticalTimer += Time.deltaTime;
    //                return;
    //            }
    //        }
    //        if (!movedImmediateVertical) movedImmediateVertical = true;
    //        verticalTimer = 0;

    //        transform.position += new Vector3(0, -1, 0);
    //        fall = Time.time;
    //        if (CheckIsValidPosition())
    //        {
    //            FindObjectOfType<Game>().UpdateGrid(this);
    //            if (Input.GetKeyDown(KeyCode.DownArrow))
    //            {
    //                PlayMoveAudio();
    //            }
    //        }
    //        else
    //        {
    //            transform.position += new Vector3(0, 1, 0);
    //            PlayLandAudio();
    //            bool check = FindObjectOfType<Game>().DeleteRow();
    //            if (check) PlayLineClearedAudio();
    //            Game.currentScore += individualScore >> 3;

    //            if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
    //            {
    //                FindObjectOfType<Game>().GameOver();
    //            }

    //            enabled = false;
    //            FindObjectOfType<Game>().SpawnNextTetromino();
    //        }
    //        fall = Time.time;
    //    }

    //}

    void PlayMoveAudio()
    {
        audioSource.PlayOneShot(moveSound);
    }
    void PlayRotateAudio()
    {
        audioSource.PlayOneShot(rotateSound);
    }
    void PlayLandAudio()
    {
        audioSource.PlayOneShot(landSound);
    }
    void PlayLineClearedAudio()
    {
        audioSource.PlayOneShot(lineClearedSound);
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
            if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false)
            {
                return false;
            }
            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }
}
