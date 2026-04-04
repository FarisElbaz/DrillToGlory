using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Linq;
using UnityEngine.InputSystem.XR.Haptics;

public class CardViews : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler 
{
    private HandView handView;
    private RectTransform cardRect;
    private RectTransform parentRect;
    private Canvas parentCanvas;
    private Vector2 dragOffset;

    private Vector3 basePosition;
    private Quaternion baseRotation; 
    private int baseZIndex;

    [SerializeField] private RectTransform dropZone;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI manaCost;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI description;

    public CardData cardData { get; private set; }

    private void Awake()
    {
        cardRect = transform as RectTransform;
        parentRect = cardRect != null ? cardRect.parent as RectTransform : null;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }


    public void SetHandView(HandView handViewRef)
    {
        handView = handViewRef;
    }

    public void injection(CardData data, RectTransform dropZoneRef, HandView handViewRef)
    {
        if (data == null) 
        {
            Debug.LogError("FAIL: The manager passed an empty data file!");
            return; 
        }
        if (cardName == null) 
        {
            Debug.LogError("FAIL: The Text UI is NOT plugged into the Inspector on this card!");
            return; 
        }

        cardData = data;
        dropZone = dropZoneRef;
        SetHandView(handViewRef);
        Debug.Log("SUCCESS: Injecting data for: " + data.cardName);


        cardName.text = cardData.cardName;
        manaCost.text = cardData.manaCost.ToString();
        damage.text = cardData.damage.ToString();
        defense.text = cardData.defense.ToString();
        description.text = cardData.description;
    }
    public void SetBaseTargets(Vector3 targetPos, float targetRotZ, int zIndex) 
    {
        basePosition = targetPos;
        baseRotation = Quaternion.Euler(0f, 0f, targetRotZ); 
        baseZIndex = zIndex;
        
        SendToBase();
    }

    public void SendToBase() 
    {
        transform.DOKill(); 
        
        transform.DOLocalMove(basePosition, 0.25f).SetEase(Ease.OutCubic);
        transform.DOLocalRotateQuaternion(baseRotation, 0.25f).SetEase(Ease.OutCubic);
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutCubic);
        
        transform.SetSiblingIndex(baseZIndex);
    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        Debug.Log("MOUSE SUCCESSFULLY CLICKED THE CARD!");
        transform.DOKill();
        transform.SetAsLastSibling(); 

        if (cardRect != null && parentRect != null)
        {
            Camera eventCamera = eventData.pressEventCamera;
            if (eventCamera == null && parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                eventCamera = parentCanvas.worldCamera;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventCamera, out Vector2 localPoint))
            {
                dragOffset = cardRect.anchoredPosition - localPoint;
            }
            else
            {
                dragOffset = Vector2.zero;
            }
        }
        if (handView.CurrentState != HandView.GameState.PlayerTurn)
        {
            return;
        }
        else
        {
            dragOffset = Vector2.zero;
        }

        Vector3 popUpPos = basePosition + new Vector3(0, 100f, 0);
        transform.DOLocalMove(popUpPos, 0.15f).SetEase(Ease.OutBack);

        transform.DOScale(Vector3.one * 1.2f, 0.15f).SetEase(Ease.OutBack);
        transform.DOLocalRotateQuaternion(Quaternion.identity, 0.15f).SetEase(Ease.OutQuad);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.DOKill();

        if (handView.CurrentState != HandView.GameState.PlayerTurn)
        {
            return;
        }

        if (cardRect == null || parentRect == null)
        {
            return;
        }

        Camera eventCamera = eventData.pressEventCamera;
        if (eventCamera == null && parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = parentCanvas.worldCamera;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventCamera, out Vector2 localPoint))
        {
            cardRect.anchoredPosition = localPoint + dragOffset;
        }
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        if (dropZone == null || handView == null)
        {
            SendToBase();
            return;
        }

        Enemy target = handView.Enemies.FirstOrDefault(e =>
        {
            if (e == null || !e.gameObject.activeInHierarchy)
            {
                return false;
            }

            RectTransform enemyRect = e.transform as RectTransform;
            if (enemyRect == null)
            {
                return false;
            }

            Camera eventCamera = eventData.pressEventCamera;
            if (eventCamera == null && parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                eventCamera = parentCanvas.worldCamera;
            }

            return RectTransformUtility.RectangleContainsScreenPoint(enemyRect, eventData.position, eventCamera);
        });

        if (target != null)
        {
            handView.OnCardPlayed(this, target);
            return;
        }

        SendToBase();
    }
}