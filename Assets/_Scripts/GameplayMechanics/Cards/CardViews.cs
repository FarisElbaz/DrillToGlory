using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class CardViews : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler 
{
    private const float DragThreshold = 10f;
    private static CardViews selectedCard;

    private HandView handView;
    [SerializeField] private UiManager uiManager;

    [SerializeField] private CardDescription cardDescription;
    private RectTransform cardRect;
    private RectTransform parentRect;
    private Canvas parentCanvas;
    private Vector2 dragOffset;

    private Vector3 basePosition;
    private Vector3 baseRotation; 
    private int baseZIndex;
    private bool isDragging;
    private bool isSelected;
    Vector2 selectedPosition;

    [SerializeField] private RectTransform dropZone;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI manaCost;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image cardArt;

    public CardData cardData => _cardData;
    private CardData _cardData;

    private void Awake()
    {
        cardRect = transform as RectTransform;
        parentRect = cardRect != null ? cardRect.parent as RectTransform : null;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void OnDisable()
    {
        if (selectedCard == this)
        {
            selectedCard = null;
        }

        transform.DOKill();
    }

    private void OnDestroy()
    {
        if (selectedCard == this)
        {
            selectedCard = null;
        }

        transform.DOKill();
    }


    public void SetHandView(HandView handViewRef)
    {
        handView = handViewRef;
    }
    public void SetCardDescriptionview(CardDescription cardDescriptionRef)
    {
        cardDescription = cardDescriptionRef;
    }

    public void SetUp(CardData data, RectTransform dropZoneRef, HandView handViewRef, CardDescription cardDescriptionRef)
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

        _cardData = data;
        dropZone = dropZoneRef;
        SetHandView(handViewRef);
        SetCardDescriptionview(cardDescriptionRef);
        Debug.Log("SUCCESS: Injecting data for: " + data.cardName);

        if (uiManager != null)
        {
            uiManager.UpdateCardDisplay(cardName, manaCost, damage, defense, description, cardArt, cardData);
        }
        else
        {
            cardName.text = cardData.cardName;
            manaCost.text = cardData.manaCost.ToString();
            damage.text = cardData.damage.ToString();
            defense.text = cardData.defense.ToString();
            description.text = cardData.description;
            cardArt.sprite = cardData.cardArt;
        }
    }
    public void SetBaseTargets(Vector3 targetPos, float targetRotZ, int zIndex) 
    {
        basePosition = targetPos;
        baseRotation = new Vector3(0f, 0f, targetRotZ);
        baseZIndex = zIndex;

        if (!isDragging)
        {
            SendToBase();
        }
    }

    public void SendToBase() 
    {
        transform.DOKill(); 
        
        transform.DOLocalMove(basePosition, 0.25f).SetEase(Ease.OutCubic);
        transform.DOLocalRotate(baseRotation, 0.25f).SetEase(Ease.OutCubic);
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutCubic);
        
        transform.SetSiblingIndex(baseZIndex);
    }

    private bool IsPointerOver(RectTransform targetRect, PointerEventData eventData)
    {
        if (targetRect == null)
        {
            return false;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(targetRect, eventData.position, null);
    }

    private void SetSelectedVisual(bool selected)
    {
        isSelected = selected;
        transform.DOKill();

        if (isSelected)
        {
            transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.15f).SetEase(Ease.OutBack);
            transform.DOLocalRotate(Vector3.zero, 0.15f).SetEase(Ease.OutQuad);
            return;
        }

        SendToBase();
    }

    private void DeselectAndReturnToBase()
    {
        isDragging = false;
        SetSelectedVisual(false);

        if (selectedCard == this)
        {
            selectedCard = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        Debug.Log("MOUSE SUCCESSFULLY CLICKED THE CARD!");

        if (handView == null || handView.CurrentState != HandView.GameState.PlayerTurn)
        {
            return;
        }

        transform.DOKill();
        transform.SetAsLastSibling(); 

        if (selectedCard != null && selectedCard != this)
        {
            selectedCard.DeselectAndReturnToBase();
        }

        if (cardRect != null && parentRect != null)
        {

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, null, out Vector2 localPoint))
            {
                dragOffset = cardRect.anchoredPosition - localPoint;
            }
            else
            {
                dragOffset = Vector2.zero;
            }
        }
        selectedPosition = eventData.position;
        isDragging = false;
        selectedCard = this;

        SetSelectedVisual(true);
        cardDescription.setCurrentCard(cardData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (handView == null || handView.CurrentState != HandView.GameState.PlayerTurn)
        {
            return;
        }

        if (cardRect == null || parentRect == null)
        {
            return;
        }
        Vector2 dragDelta = eventData.position - selectedPosition;
        if (!isDragging && dragDelta.magnitude <= DragThreshold)
        {
            return;
        }

        if (!isDragging)
        {
            isDragging = true;
            transform.DOKill();
            transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetEase(Ease.OutCubic);
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, null, out Vector2 localPoint))
        {
            cardRect.anchoredPosition = localPoint + dragOffset;
        }
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        if (handView == null || handView.CurrentState != HandView.GameState.PlayerTurn)
        {
            DeselectAndReturnToBase();
            return;
        }

        if (!isDragging)
        {
            SetSelectedVisual(true);
            return;
        }

        if (dropZone == null || handView == null)
        {
            isDragging = false;
            SendToBase();
            return;
        }

        if (cardData.cardType == CardData.CardType.Attack)
        {
            var target = handView.Enemies.FirstOrDefault(e =>
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

                    return IsPointerOver(enemyRect, eventData);
                });
                if (target != null)
                {
                    isDragging = false;
                    isSelected = false;
                    if (selectedCard == this)
                    {
                        selectedCard = null;
                    }
                    handView.OnCardPlayed(this, target);
                    return;
                }
        }
        else if ((cardData.cardType == CardData.CardType.Defense || cardData.cardType == CardData.CardType.Heal) && IsPointerOver(dropZone, eventData))
        {
            isDragging = false;
            isSelected = false;
            if (selectedCard == this)
            {
                selectedCard = null;
            }
            handView.OnCardPlayed(this, null);
            return;
        }

        isDragging = false;
        SendToBase();
    }
}