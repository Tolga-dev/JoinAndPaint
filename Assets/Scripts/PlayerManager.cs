using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller.Spawners;
using GameObjects.Boss;
using TMPro;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Managers")] public GameManager gameManager;

    [Header("Controllers")] public InputController inputController;
    public PlayerAnimationController animationController;

    [Header("Visual")] public Recruitment recruitment;

    [Header("Parameters")] public float xSpeed = 15f;
    public float zSpeed = 10f;
    public float rotationSpeed;

    public List<Recruitment> members = new List<Recruitment>();
    public Vector3 direction;
    public Transform prizeEffectSpawnPoint;
    public Transform canvasSpawnPoint;

    public bool memberEditMode = false;
    public float maxDistanceMove = 0.5f;
    public float maxMergeDistanceMove;
    public Transform mergePos;

    public float scaleBigFactor;

    public Transform accessorLeftHand;
    public Transform accessorRightHand;
    public Transform accessorHead;
    public List<GameObject> accessories = new List<GameObject>();

    public GameObject canvas;
    public TextMeshProUGUI healthUI;
    public Slider slider;
    
    private void Start()
    {
        members.Add(recruitment);
        PlayerUiUpdate();
    }


    public void UpdatePlayer()
    {
        inputController.HandleMouseInput();

        if (memberEditMode)
            return;
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (inputController.canMove == false)
        {
            foreach (var member in members)
            {
                member.rb.velocity = Vector3.zero;
                member.playerAnimationController.SetPlayerIdle();
            }
        }
        else
        {
            SetDirection();
            SetRot();

            var velocity = new Vector3(direction.x * xSpeed, 0f, direction.z * zSpeed);
            foreach (var member in members)
            {
                member.rb.velocity = velocity;
                member.playerAnimationController.StartRunner();
            }
        }
    }

    private void SetRot()
    {
        foreach (var member in members)
        {

            member.transform.rotation =
                Quaternion.Slerp(member.transform.rotation, member.rb.velocity.magnitude > 0.5f
                    ? Quaternion.LookRotation(member.rb.velocity)
                    : Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }

    private void SetDirection()
    {
        direction.x = Mathf.Lerp(direction.x, inputController.IsMouseX(), Time.deltaTime * xSpeed);
        direction.z = 1;

        direction = Vector3.ClampMagnitude(direction, 1f);
    }


    public void ResetPlayer()
    {
        recruitment.UnFreeze();
        gameObject.SetActive(true);

        ResetPos();
        animationController.Reset();
        ResetInput();

        foreach (var member in members)
        {
            if (member == recruitment)
                continue;
            Destroy(member);
        }

        members.Clear();

        members.Add(recruitment);
        recruitment.rb.velocity = Vector3.zero;

        
        SpawnAndDestroyAccessories();

        recruitment.health = gameManager.gamePropertiesInSave.maxHealth;
        
        PlayerUiUpdate();
        canvas.SetActive(false);

    }

    private void SpawnAndDestroyAccessories()
    {
        var currentLevel = gameManager.gamePropertiesInSave.currenLevel;

        // Destroy old accessories
        foreach (var accessory in accessories)
        {
            Destroy(accessory);
        }

        accessories.Clear();

        var accessorTypes = gameManager.spawnerManager.prizeSpawner.accessorTypes;

        // Spawn new accessories based on the current level
        if (currentLevel >= 0 && currentLevel <= 25)
        {
            // Spawn Head accessory only
            SpawnAccessory(AccessorTypesEnum.Head, accessorTypes);
        }
        else if (currentLevel > 25 && currentLevel <= 50)
        {
            // Spawn Head and Left Hand accessories
            SpawnAccessory(AccessorTypesEnum.Head, accessorTypes);
            SpawnAccessory(AccessorTypesEnum.LeftHand, accessorTypes);
        }
        else if (currentLevel > 50)
        {
            // Spawn Head, Left Hand, and Right Hand accessories
            SpawnAccessory(AccessorTypesEnum.Head, accessorTypes);
            SpawnAccessory(AccessorTypesEnum.LeftHand, accessorTypes);
            SpawnAccessory(AccessorTypesEnum.RightHand, accessorTypes);
        }
    }

    private void SpawnAccessory(AccessorTypesEnum accessoryType, AccessorTypes accessorTypes)
    {
        // Select a random accessory type and material
        var randomAccessorType = accessorTypes.accessorTypes.First(a => a.accessorType == accessoryType);
        var accessor = randomAccessorType.accessories[Random.Range(0, randomAccessorType.accessories.Count)];
        var currentAccessor = accessor.accessor;

        var createdRecruitment = recruitment;
        createdRecruitment.damageAmount += accessor.power + gameManager.gamePropertiesInSave.updateAmount;

        // Spawn the accessory at the appropriate position
        Transform parentTransform = null;

        switch (accessoryType)
        {
            case AccessorTypesEnum.Head:
                parentTransform = accessorHead;
                break;
            case AccessorTypesEnum.LeftHand:
                parentTransform = accessorLeftHand;
                break;
            case AccessorTypesEnum.RightHand:
                parentTransform = accessorRightHand;
                break;
        }

        if (parentTransform != null)
        {
            var createdAccessor = Object.Instantiate(currentAccessor, parentTransform);
            createdAccessor.transform.localPosition = Vector3.zero;
            createdAccessor.GetComponent<MeshRenderer>().material =
                accessorTypes.materials[Random.Range(0, accessorTypes.materials.Count)];

            // Add the spawned accessory to the accessories list
            accessories.Add(createdAccessor.gameObject);
        }
    }

    public void ResetInput()
    {
        inputController.isMouseDown = false;
        inputController.canMove = false;
    }

    public void ResetPos()
    {
        var initPos = gameManager.playingState.playerInitialPosition;
        transform.position = initPos.position;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
    }

    public void SetWin()
    {
        recruitment.rb.velocity = Vector3.zero;
        gameManager.playingState.isGameFinished = true;

        if (gameManager.playingState.score > 0 && gameManager.playingState.isGameWon)
        {
            gameManager.gamePropertiesInSave.currenLevel++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Recruitment"))
        {
            var rec = other.transform.GetComponent<Recruitment>();
            if (rec.isHitPlayer)
                return;
            gameManager.memberManager.AddNewMember(rec);
        }
    }

    public void GotHitReaction()
    {
        Debug.Log("die");
    }

    public void TargetToATransform(Boss target, bool isAttack) // hated ofc but, i was bored tbh
    {
        if (isAttack)
        {
            foreach (var member in members)
            {
                var rb = member.rb;

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.drag = 0f;
                rb.angularDrag = 0f;

                member.Attack(member, target);
            }

        }
        else
        {



            StartCoroutine(AttackToTarget(target));
        }

    }

    public IEnumerator AttackToTarget(Boss target)
    {
        yield return MergeMembers();

        if (members.Count > 1)
        {
            bool isAllMembersAreMerged = false;
            while (!isAllMembersAreMerged)
            {
                isAllMembersAreMerged = true;
                foreach (var member in members)
                {
                    if (member == recruitment)
                        continue;

                    if (member.merged == false)
                    {
                        isAllMembersAreMerged = false;
                        break; // Exit the loop once you find an unmerged member
                    }
                }

                // Optionally, add a small delay between each check to avoid continuous looping
                yield return null;
            }
        }

        // Now that all members are merged, perform the attack
        recruitment.Attack(recruitment, target);
    }


    public IEnumerator MergeMembers()
    {
        List<Coroutine> mergeCoroutines = new List<Coroutine>();

        foreach (var member in members)
        {
            var rb = member.rb;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.drag = 0f;
            rb.angularDrag = 0f;

            if (members.Count == 1)
                break;

            if (member == recruitment)
                continue;

            Coroutine mergeCoroutine = StartCoroutine(member.MergeCoroutine(member, transform));
            mergeCoroutines.Add(mergeCoroutine);
        }

        var incMe = members.Count / 20;
        maxMergeDistanceMove += incMe * 0.1f;

        foreach (var mergeCoroutine in mergeCoroutines)
        {
            yield return mergeCoroutine;
        }

        maxMergeDistanceMove -= incMe * 0.1f;

    }

    public void PlayerUiUpdate()
    {
        canvas.SetActive(true);
        
        var health = recruitment.health;
        var maxHealth = gameManager.gamePropertiesInSave.maxHealth;
        
        if (health < 0)
        {
            slider.value = 0;
            healthUI.text = 0.ToString();
            return;
        }
            
        slider.value = (float)health / maxHealth;
        healthUI.text = health.ToString();
    }

}