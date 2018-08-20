using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	public bool canMove;


	private bool isDead;
	public bool IsDead
	{
		get { return isDead; }
	}

	public int RemainingBirdFlaps
	{
		get { return remainingBirdFlaps; }
	}

	//BASIC
	public float MovementSpeed = 4;
	public float JumpStrength = 8;
	public float FallDeathY = -4;
	public float FallDeathTime = 3;

	//SOUNDS
	public AudioClip JumpFall;
	public AudioClip TimeWarp;
	public AudioClip SpiritForm;

	//BIRD
	public bool BirdMechanics;
	public float FlapStrengthX;
	public float FlapStrengthY;
	public int MaxBirdFlaps;

	//REWIND
	public bool RewindMechanics;
	public Transform PastSelfPrefab;
	public int RewindLength;
	public int RewindRecordPerSecond;

	//SPIRIT
	public bool SpiritMechanics;
	public float SpiritFormDuration;

	//ANIMS
	public float IdleFrameTime;
	public List<Sprite> IdleFrames;
	public float WalkFrameTime;
	public List<Sprite> WalkFrames;
	public float BirdFrameTime;
	public List<Sprite> BirdFrames;
	public float SpiritFrameTime;
	public List<Sprite> SpiritFrames;

	//PRIVATES -------------------------------

	//basic
	private bool facingRight = true;
	private bool isWalking = false;
	private Rigidbody2D rbody;
	private float fallDeathTimer = -1;
	private float jumpCd = 0.1f;
	private float useCd = 0.1f;

	private AudioSource audioSource;

	//animations
	private new SpriteRenderer renderer;
	private float animationTimer = -1;
	private int frameID = 0;


	//bird
	private bool birdForm;
	private int remainingBirdFlaps;

	//rewind
	private List<Vector2> rewindPositions;
	private List<Sprite> rewindSprites;
	private List<bool> rewindFacingRights;
	private float rewindRecordTimer;
	private Transform pastSelf;
	private SpriteRenderer pastSelfRenderer;

	//spirit
	private float spiritRemaining = -1;

	public void Die()
	{
		isDead = true;
		//todo: hangeffekt?
	}

	void Start()
	{
		Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
		rbody = GetComponent<Rigidbody2D>();
		renderer = GetComponent<SpriteRenderer>();
		audioSource = GetComponent<AudioSource>();
		rewindPositions = new List<Vector2>();
		rewindSprites = new List<Sprite>();
		rewindFacingRights = new List<bool>();
		if (RewindMechanics)
		{
			pastSelf = Instantiate(PastSelfPrefab, transform.position, Quaternion.identity);
			pastSelfRenderer = pastSelf.GetComponent<SpriteRenderer>();
			InitializeRewindList();
			rewindRecordTimer = 1.0f / RewindRecordPerSecond;
			useCd = RewindLength;
		}

	}

	void FixedUpdate()
	{
		if (canMove == false) return;

		if (jumpCd > 0)
		{
			jumpCd -= Time.deltaTime;
		}

		var velocity = rbody.velocity;

		//move
		float input = Input.GetAxis("Horizontal");
		if(!IsAtWall())
		{
			velocity.x = input * MovementSpeed;
			if(input > 0)
			{
				facingRight = true;
				renderer.flipX = false;
			}
			else if(input < 0)
			{
				facingRight = false;
				renderer.flipX = true;
			}
		}

		if(Mathf.Abs(input) > 0.1f && (renderer.sprite == IdleFrames[0] || renderer.sprite == IdleFrames[1])) //this will fuck up some things if idle anim is more than 2 frames
		{
			animationTimer = -1; //reset idle anim timer
		}

		//jump
		if (Input.GetKeyDown(KeyCode.Space) && IsAtGround() && jumpCd < 0.001f)
		{
			//Debug.Log("Jump");
			velocity.y += JumpStrength;
			jumpCd = 0.1f;
			audioSource.PlayOneShot(JumpFall);
		}

		rbody.velocity = velocity;

		if(fallDeathTimer < 0) //not started dying due to fall
		{
			Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
		}

		HandleAnimations();

		HandleMechanics();

		CheckFallDeath();
	}

	private bool IsAtWall()
	{
		//Debug.DrawLine(transform.position + new Vector3(0, 0.5f, 0), transform.position + new Vector3(0, 0.5f, 0) - 0.5f * transform.right, Color.red);
		//Debug.DrawLine(transform.position - new Vector3(0, 0.5f, 0), transform.position + new Vector3(0, 0.5f, 0) - 0.5f * transform.right, Color.red);
		var hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f, 0), -transform.right, 0.5f, 1 << LayerMask.NameToLayer("Platform"));
		var hit2 = Physics2D.Raycast(transform.position - new Vector3(0, 0.5f, 0), -transform.right, 0.5f, 1 << LayerMask.NameToLayer("Platform"));
		if (hit.collider != null || hit2.collider != null)
		{
			//Debug.Log("IsAtWall At Left");
			return true;
		}
		else
		{
			//Debug.DrawLine(transform.position + new Vector3(0, 0.5f, 0), transform.position + new Vector3(0, 0.5f, 0) + 0.5f * transform.right, Color.red);
			//Debug.DrawLine(transform.position - new Vector3(0, 0.5f, 0), transform.position + new Vector3(0, 0.5f, 0) + 0.5f * transform.right, Color.red);
			hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.right, 0.5f, 1 << LayerMask.NameToLayer("Platform"));
			hit2 = Physics2D.Raycast(transform.position - new Vector3(0, 0.5f, 0), transform.right, 0.5f, 1 << LayerMask.NameToLayer("Platform"));
			if (hit.collider != null || hit2.collider != null)
			{
				//Debug.Log("IsAtWall At Right");
				return true;
			}
			else
			{
				//Debug.Log("No Wall");
				return false;
			}
		}
	}

	private bool IsAtGround()
	{
		var hit = Physics2D.Raycast(transform.position, -transform.up, 1f, 1 << LayerMask.NameToLayer("Platform"));
		if (hit.collider != null)
		{
			return true;
		}
		else return false;
	}

	private void HandleAnimations()
	{
		List<Sprite> frameList = null;
		float frameTime = 0.1f;
		if(birdForm)
		{
			frameList = BirdFrames;
			frameTime = BirdFrameTime;
		}
		else if(spiritRemaining > 0)
		{
			frameList = SpiritFrames;
			frameTime = SpiritFrameTime;
		}
		else if(rbody.velocity.magnitude > 0.5f) //IF WALKING GOES WRONG THIS MAYBE CAUSES IT
		{
			frameList = WalkFrames;
			frameTime = WalkFrameTime;
		}
		else
		{
			frameList = IdleFrames;
			frameTime = IdleFrameTime;
		}

		animationTimer -= Time.deltaTime;
		if (animationTimer < 0)
		{
			renderer.sprite = frameList[frameID++];
			if (frameID == frameList.Count)
			{
				frameID = 0;
			}
			animationTimer = frameTime;
		}
	}

	private void HandleMechanics()
	{
		if(useCd > 0)
		{
			useCd -= Time.deltaTime;
		}
		if (BirdMechanics)
		{
			HandleBirdMechanics();
		}
		if(RewindMechanics)
		{
			HandleRewindMechanics();
		}
		if (SpiritMechanics)
		{
			HandleSpiritMechanics();
		}
	}

	private void HandleBirdMechanics()
	{
		if (birdForm)
		{
			if (Input.GetKeyDown(KeyCode.Space) && remainingBirdFlaps > 0 && jumpCd < 0.001f)
			{
				FlapWings();
			}
			if(IsAtGround())
			{
				birdForm = false;
				frameID = 0;
				audioSource.PlayOneShot(JumpFall);
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Space) && jumpCd < 0.001f)
			{
				birdForm = true;
				frameID = 0;
				remainingBirdFlaps = MaxBirdFlaps + 1;
				audioSource.PlayOneShot(JumpFall);
				FlapWings();
			}
		}

	}

	private void FlapWings()
	{
		//rbody.AddForce(new Vector2(facingRight ? FlapStrengthX: -FlapStrengthX, FlapStrengthY));
		rbody.velocity = new Vector2(facingRight ? FlapStrengthX : -FlapStrengthX, FlapStrengthY);
		remainingBirdFlaps--;
		audioSource.PlayOneShot(JumpFall);
	}

	private void HandleRewindMechanics()
	{
		rewindRecordTimer -= Time.deltaTime;
		if(rewindRecordTimer < 0.0001f)
		{
			rewindPositions.RemoveAt(0);
			rewindPositions.Add(transform.position);
			rewindSprites.RemoveAt(0);
			rewindSprites.Add(renderer.sprite);
			rewindFacingRights.RemoveAt(0);
			rewindFacingRights.Add(facingRight);
			rewindRecordTimer = 1.0f / RewindRecordPerSecond;
			pastSelf.position = rewindPositions[0];
			pastSelfRenderer.sprite = rewindSprites[0];
			pastSelfRenderer.flipX = !rewindFacingRights[0];
		}

		if (Input.GetKeyDown(KeyCode.E) && useCd < 0.001f)
		{
			transform.position = rewindPositions[0];
			renderer.sprite = rewindSprites[0];
			renderer.flipX = !rewindFacingRights[0];
			//todo: play anim at locations
			useCd = RewindLength;
			InitializeRewindList();
			audioSource.PlayOneShot(TimeWarp);
		}
	}

	private void InitializeRewindList()
	{
		rewindPositions.Clear();
		rewindSprites.Clear();
		for(int s = 0; s < RewindLength; s++)
		{
			for(int i = 0; i < RewindRecordPerSecond; i++)
			{
				rewindPositions.Add(transform.position);
				rewindSprites.Add(IdleFrames[s % 2]);
				rewindFacingRights.Add(facingRight);
			}
		}
	}

	private void HandleSpiritMechanics()
	{
		if(spiritRemaining > 0)
		{
			spiritRemaining -= Time.deltaTime;
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				transform.position = transform.position + new Vector3(0, MovementSpeed * Time.deltaTime, 0);
			}
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				transform.position = transform.position - new Vector3(0, MovementSpeed * Time.deltaTime, 0);
			}
			if (spiritRemaining < 0)
			{
				rbody.isKinematic = false;
				frameID = 0;
				useCd = 1f;
			}
		}
		else
		{
			if(Input.GetKeyDown(KeyCode.E) && useCd < 0.001f)
			{
				rbody.isKinematic = true;
				frameID = 0;
				rbody.velocity = new Vector2(rbody.velocity.x, 0);
				spiritRemaining = SpiritFormDuration;
				audioSource.PlayOneShot(SpiritForm);
			}
		}
	}

	private void CheckFallDeath()
	{
		if(fallDeathTimer < 0) //not started dying due to falling yet
		{
			if (transform.position.y < FallDeathY)
			{
				fallDeathTimer = FallDeathTime;
			}
		}
		else
		{
			fallDeathTimer -= Time.deltaTime;
			if(fallDeathTimer < 0.001f)
			{
				Die();
			}
		}
	}
}
