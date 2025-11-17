using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro; 

public class throwingscript : MonoBehaviour
{
    public List<GameObject> throwingObjects = new List<GameObject>();
    public float throwForce = 30f;
    public float throwTime = 0.42f;
    public float destroyTime = 0.75f;

    private GameObject moveThrowingObject;
    public GameObject throwButton;//for playing animations and off button
                                  //  public Button throwButton;


    public Animator staminaAnimator;
    
    public AudioSource throwSound;
    //public AudioSource PainSound;
    //private bool throwSound = false;

    // private bool StaminaAnime = true;
    public TMP_Text bananaCountField;
    public int bananaCount =0;
   

    public void Start()
    {
        // bananaCountField.text = bananaCount.ToString();

        //throwButton.onClick.AddListener(() =>//////////
        //{
        //    staminaAnimator.SetTrigger("Refills");//animation part

        //    throwButton.gameObject.SetActive(false);

        //    Invoke("ThrowObjectFun", throwTime);
        //    Invoke("ButtonEnableTime", throwTime+0.5f);
        //});
    }

   public void OnThrowButtonClicked()
    {
        if (bananaCount > 0)
        {

    bananaCount = bananaCount-1;
            bananaCountField.text = bananaCount.ToString();
            staminaAnimator.SetTrigger("Refills"); // animation part
            throwButton.gameObject.SetActive(false);
            Invoke("ThrowObjectFun", throwTime);
            Invoke("ButtonEnableTime", throwTime + 0.5f);
        }
    }


    public void ThrowObjectFun()
    {
     
        Instantiate(throwingObjects[0], transform.position, Quaternion.identity);
        moveThrowingObject = GameObject.FindWithTag("throwingObject");
        throwButton.gameObject.SetActive(false);
        throwSound.Play();
       // throwSound.enabled= true ;

    }
    public void ButtonEnableTime()
    {
       
    throwButton.gameObject.SetActive(true);
      //  throwSound.enabled = false;

    }
    public void Update()
    {
        bananaCountField.text = bananaCount.ToString();
  
    }

    public void FixedUpdate()
    {
        if (moveThrowingObject != null)
        {
            moveThrowingObject.transform.Translate(Vector3.forward * throwForce * Time.deltaTime);
            Destroy(moveThrowingObject, destroyTime);
        }
    }
  
}
