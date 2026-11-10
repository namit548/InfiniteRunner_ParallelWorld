//using UnityEditor.Rendering;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float laneDistance = 3f;
    [SerializeField, Min(0.01f)] private float laneChangeDuration = 0.25f;
    private float firstLaneX;
    public int laneNumber = 1;
    public float force = 10f;


    //public float minX = -3f, maxX = 3f;

    private Rigidbody _rb;

    public bool isJumped = false;
    public GameObject gameOverPanel;


    private Animator myAnim;//
    bool isRunning  = false;
    bool isJumping = false;
    bool isThrowAni = false;

    //  public AudioSource ;
    public AudioSource GaliSound, BackgroundSOund;


    // Swipe
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    public void Start()
    {
        GaliSound.Play();
        myAnim = GetComponent<Animator>();//
        _rb = GetComponent<Rigidbody>();
        firstLaneX = _rb.position.x - laneNumber * laneDistance;
        gameOverPanel.SetActive(false);
        BackgroundSOund.enabled = true;
    }
    private void Update()
    {

       // isRunning = Input.GetKey(KeyCode.UpArrow);//
        //isThrowAni = Input.GetMouseButtonDown(0);//bug here i.e not plasying by button but by click
        myAnim.SetBool("Running", isRunning);//
        myAnim.SetBool("Jump", isJumping);//
        myAnim.SetBool("Throw", isThrowAni);//

        if (Input.GetKeyDown(KeyCode.LeftArrow) && laneNumber >= 1)
        {
            SwipeLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && laneNumber <= 1)
        {
            SwipeRight();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!isJumped)
            {
                _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
                isJumped = true;
                isJumping = true;
                isRunning = true;
            }

        }






        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPosition = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endTouchPosition = Input.GetTouch(0).position;

            Vector2 inputVector = endTouchPosition - startTouchPosition;

            if (Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.y))
            {
                if (inputVector.x > 0)
                    SwipeRight();
                else
                    SwipeLeft();
            }
            else
            {
                if (inputVector.y > 0)

                    SwipeUp();
            }
        }

    }


    //private void DetectSwipe()
    //{
    //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    //    {
    //        startTouchPosition = Input.GetTouch(0).position;
    //    }

    //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
    //    {
    //        endTouchPosition = Input.GetTouch(0).position;

    //        Vector2 inputVector = endTouchPosition - startTouchPosition;

    //        if (Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.y))
    //        {
    //            if (inputVector.x > 0)
    //                SwipeRight();
    //            else
    //                SwipeLeft();
    //        }
    //        else
    //        {
    //            if (inputVector.y > 0)
    //                SwipeUp();
    //        }
    //    }
    //}



    private void FixedUpdate()
    {
        float targetX = firstLaneX + laneNumber * laneDistance;
        float speed = Mathf.Abs(laneDistance) / laneChangeDuration;
        float nextX = Mathf.MoveTowards(_rb.position.x, targetX, speed * Time.fixedDeltaTime);
        Vector3 velocity = _rb.linearVelocity;
        velocity.x = (nextX - _rb.position.x) / Time.fixedDeltaTime;
        _rb.linearVelocity = velocity;
    }

    public void SwipeLeft()
    {
        laneNumber = Mathf.Max(0, laneNumber - 1);
    }

    public void SwipeRight()
    {
        laneNumber = Mathf.Min(2, laneNumber + 1);
    }

    public void SwipeUp()
    {
        if (!isJumped   )
        {
            _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            isJumped = true;
           // isJumping = true;
            isRunning = true;
        }
    }


    public void PlayThrowingAni()//Enable animation
    {
        isThrowAni = true;
        Invoke("pauseThrowingAni", .3f);

    }
    public void pauseThrowingAni()//Enable animation
    {
        isThrowAni = false;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isJumped = false;
            isJumping = true;
            isRunning = false;


        }
        else if (collision.gameObject.tag == "Collectable")// for throwingobject ammo type
        {
            Debug.Log("Banana mil gya");
          //  throwingscript f = GameObject.<throwingscript>();
           throwingscript thrower = GameObject.FindObjectOfType<throwingscript>();//confusing
            thrower.bananaCount += 1;
            Destroy(collision.gameObject);

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("Game Over");
            Time.timeScale = 0f;
            BackgroundSOund.enabled = false;
            gameOverPanel.SetActive(true);
        }

    }



}




































//using Unity.VisualScripting;
//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    public float laneDistance = 3f;
//    public int laneNumber = 1;
//    public float force = 10f;

//    private Rigidbody _rb;
//    public bool isJumped = false;
//    public GameObject gameOverPanel;

//    private Animator myAnim;
//    bool isRunning = false;
//    bool isJumping = false;
//    bool isThrowAni = false;

//    public AudioSource GaliSound, BackgroundSOund;

//    private Vector2 startTouchPosition;
//    private Vector2 endTouchPosition;

//    public void Start()
//    {
//        GaliSound.Play();
//        myAnim = GetComponent<Animator>();
//        _rb = GetComponent<Rigidbody>();
//        gameOverPanel.SetActive(false);
//        BackgroundSOund.enabled = true;
//    }

//    private void Update()
//    {
//        isRunning = Input.GetKey(KeyCode.UpArrow);
//        myAnim.SetBool("Running", isRunning);
//        myAnim.SetBool("Throw", isThrowAni);

//        // Keyboard input for left, right, jump
//        if (Input.GetKeyDown(KeyCode.LeftArrow) && laneNumber >= 1)
//        {
//            transform.position = new Vector3(transform.position.x - laneDistance, transform.position.y, transform.position.z);
//            laneNumber--;
//        }
//        else if (Input.GetKeyDown(KeyCode.RightArrow) && laneNumber <= 1)
//        {
//            transform.position = new Vector3(transform.position.x + laneDistance, transform.position.y, transform.position.z);
//            laneNumber++;
//        }
//        else if (Input.GetKeyDown(KeyCode.UpArrow))
//        {
//            if (!isJumped)
//            {
//                _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
//                isJumped = true;
//                isJumping = true;
//                myAnim.SetBool("Jump", true);
//                Invoke("ResetJumpAnim", 0.5f);
//            }
//        }

//        // Touch input
//        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
//        {
//            startTouchPosition = Input.GetTouch(0).position;
//        }

//        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
//        {
//            endTouchPosition = Input.GetTouch(0).position;
//            Vector2 inputVector = endTouchPosition - startTouchPosition;

//            if (Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.y))
//            {
//                if (inputVector.x > 0)
//                    SwipeRight();
//                else
//                    SwipeLeft();
//            }
//            else
//            {
//                if (inputVector.y > 0)
//                    SwipeUp();
//            }
//        }
//    }

//    public void SwipeLeft()
//    {
//        if (laneNumber >= 1)
//        {
//            transform.position = new Vector3(transform.position.x - laneDistance, transform.position.y, transform.position.z);
//            laneNumber--;
//        }
//    }

//    public void SwipeRight()
//    {
//        if (laneNumber <= 1)
//        {
//            transform.position = new Vector3(transform.position.x + laneDistance, transform.position.y, transform.position.z);
//            laneNumber++;
//        }
//    }

//    public void SwipeUp()
//    {
//        if (!isJumped)
//        {
//            _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
//            isJumped = true;
//            isJumping = true;
//            myAnim.SetBool("Jump", true); // play jump animation
//            Invoke("ResetJumpAnim", 0.5f); // reset animation after short time
//        }
//    }

//    void ResetJumpAnim()
//    {
//        isJumping = false;
//        myAnim.SetBool("Jump", false); // stop jump animation
//    }

//    public void PlayThrowingAni()
//    {
//        isThrowAni = true;
//        Invoke("pauseThrowingAni", 0.3f);
//    }

//    public void pauseThrowingAni()
//    {
//        isThrowAni = false;
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.tag == "Ground")
//        {
//            isJumped = false;
//        }
//        else if (collision.gameObject.tag == "Collectable")
//        {
//            Debug.Log("Banana mil gya");
//            throwingscript thrower = GameObject.FindObjectOfType<throwingscript>();
//            thrower.bananaCount += 1;
//            Destroy(collision.gameObject);
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.tag == "Obstacle")
//        {
//            Debug.Log("Game Over");
//            Time.timeScale = 0f;
//            BackgroundSOund.enabled = false;
//            gameOverPanel.SetActive(true);
//        }
//    }
//}
