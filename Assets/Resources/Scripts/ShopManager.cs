using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopManager : Singleton<ShopManager>
{
    public ItemValueManager itemValueManager; // �A�C�e���̊e�l�Ǘ��p�̃X�N���v�g

    public TextMeshProUGUI mainText; // �e�L�X�g�\��
    public GameObject selectWindow; // �I���E�B���h�E
    public bool yes, no; // �I����
    public GameObject defaultButton; // �I�����\���Ńf�t�H���g�őI������{�^��

    public Image floorImage; // �t���A�w�i
    public Sprite[] floorSprite; // �w�i�摜
    public enum enumFloorSprite
    {
        HPPotion, // HP�|�[�V����
        SPPotion, // SP�|�[�V����
        ATKPotion, // �U���|�[�V����
        Num // �w�i��
    }
    public int[] getItemNumber;
    public enum enumGetItemNumber
    {
        HPPotion,
        SPPotion,
        ATKPotion,
        Num
    }
    public int getItem;

    public void Start()
    {
        // ScriptableObject�̓ǂݍ���
        itemValueManager = Resources.Load<ItemValueManager>("ScriptableObject/ItemValueManager");

        // �e�z��̏�����
        floorSprite = new Sprite[(int)enumFloorSprite.Num];
        getItemNumber = new int[(int)enumGetItemNumber.Num];

        // �eUI�I�u�W�F�N�g�̓ǂݍ���
        mainText = GameObject.Find("MainText").GetComponent<TextMeshProUGUI>();
        selectWindow = GameObject.Find("ShopSelectWindow");
        defaultButton = GameObject.Find("ShopSelectYes");
        floorImage = GameObject.Find("FloorImage").GetComponent<Image>();
        floorSprite[(int)enumFloorSprite.HPPotion] = Resources.Load<Sprite>("Images/FloorBacks/HPPotion");
        floorSprite[(int)enumFloorSprite.SPPotion] = Resources.Load<Sprite>("Images/FloorBacks/SPPotion");
        floorSprite[(int)enumFloorSprite.ATKPotion] = Resources.Load<Sprite>("Images/FloorBacks/ATKPotion");

        // �f�[�^���X�g����̃A�C�e��ID�̓ǂݍ���
        getItemNumber[(int)enumGetItemNumber.HPPotion] = itemValueManager.DataList[0].itemID;
        getItemNumber[(int)enumGetItemNumber.SPPotion] = itemValueManager.DataList[1].itemID;
        getItemNumber[(int)enumGetItemNumber.ATKPotion] = itemValueManager.DataList[2].itemID;

        // ������\���E�B���h�E���\��
        selectWindow.SetActive(false);

    }

    // HP�|�[�V����
    public IEnumerator HPShop()
    {
        floorImage.sprite = floorSprite[(int)enumFloorSprite.HPPotion];
        getItem = getItemNumber[(int)enumGetItemNumber.HPPotion];
        yield return StartCoroutine(PotionShop());
    }

    // SP�|�[�V����
    public IEnumerator SPShop()
    {
        floorImage.sprite = floorSprite[(int)enumFloorSprite.SPPotion];
        getItem = getItemNumber[(int)enumGetItemNumber.SPPotion];
        yield return StartCoroutine(PotionShop());
    }

    // �U���|�[�V����
    public IEnumerator ATKShop()
    {
        floorImage.sprite = floorSprite[(int)enumFloorSprite.ATKPotion];
        getItem = getItemNumber[(int)enumGetItemNumber.ATKPotion];
        yield return StartCoroutine(PotionShop());
    }

    // �V���b�v�̏���
    public IEnumerator PotionShop()
    {
        mainText.text = "��������Ⴂ�I\n�����̓|�[�V�����V���b�v����";

        yield return StartCoroutine(NextProcess(1.0f));

        SoundManager.Instance.PlaySE((int)SoundManager.enumSENumber.Select);
        mainText.text = "HP��10�����Ă��ꂽ��\n�|�[�V�����𔄂��Ă����悤";

        yield return StartCoroutine(NextProcess(1.0f));

        SoundManager.Instance.PlaySE((int)SoundManager.enumSENumber.Select);
        if (BattleManager.Instance.playerHP <= 10)
        {
            mainText.text = "�����ƁAHP������Ȃ���\n�܂����Ă�����";
        }
        else
        {
            mainText.text = $"{itemValueManager.DataList[getItem].itemName}���K�v�����H";

            yield return new WaitForSeconds(0.5f);

            selectWindow.SetActive(true);
            EventSystem.current.SetSelectedGameObject(defaultButton);

            while (!yes && !no)
            {
                yield return null;
            }
            SoundManager.Instance.PlaySE((int)SoundManager.enumSENumber.Select);
            selectWindow.SetActive(false);

            if (yes)
            {
                yes = false;

                if (ItemManager.Instance.haveItem)
                {
                    mainText.text = "�����A�C�e���������Ă��\n�A�C�e�����������邩���H";

                    yield return new WaitForSeconds(0.5f);

                    selectWindow.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(defaultButton);

                    while (!yes && !no)
                    {
                        yield return null;
                    }
                    selectWindow.SetActive(false);

                    if (yes)
                    {
                        SoundManager.Instance.PlaySE((int)SoundManager.enumSENumber.Damage);
                        FlashManager.Instance.FlashScreen(Color.red, 0.3f);
                        yes = false;
                        mainText.text = "����������\n�܂����Ƃ�����";
                        BattleManager.Instance.playerHP -= 10;
                        for (int i = 0; i < ItemManager.Instance.getItem.Length; i++)
                        {
                            ItemManager.Instance.getItem[i] = false;
                        }
                        ItemManager.Instance.getItem[getItem] = true;
                        ItemManager.Instance.itemText.text = itemValueManager.DataList[getItem].itemName;
                    }
                    else
                    {
                        SoundManager.Instance.PlaySE((int)SoundManager.enumSENumber.Select);
                        no = false;
                        mainText.text = "������Ȃ����Ƃ��肤��";
                    }

                }
                else
                {
                    SoundManager.Instance.PlaySE((int)SoundManager.enumSENumber.Damage);
                    FlashManager.Instance.FlashScreen(Color.red, 0.3f);
                    mainText.text = "����������\n�܂����Ƃ�����";
                    BattleManager.Instance.playerHP -= 10;
                    ItemManager.Instance.getItem[getItem] = true;
                    ItemManager.Instance.haveItem = true;
                    ItemManager.Instance.itemText.text = itemValueManager.DataList[getItem].itemName;
                }

            }
            else
            {
                SoundManager.Instance.PlaySE((int)SoundManager.enumSENumber.Select);
                no = false;
                mainText.text = "������Ȃ����Ƃ��肤��";
            }
        }

        yield return StartCoroutine(NextProcess(1.0f));

        SoundManager.Instance.PlaySE((int)SoundManager.enumSENumber.Select);
        SoundManager.Instance.StopBGM();
    }

    // �R���[�`�����Ŏ��̏����Ɉړ�����ۂ̃f�B���C�̐ݒ�
    public IEnumerator NextProcess(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
    }

    // �w�����̑I�����͂�
    public void Yes()
    {
        yes = true;
    }

    // �w�����̑I����������
    public void No()
    {
        no = true;
    }

}