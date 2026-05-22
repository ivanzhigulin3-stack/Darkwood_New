using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 2f;
    public LayerMask interactableLayer;

    private IInteractable currentInteractable;
    private InteractionUI interactionUI;

    void Start()
    {
        interactionUI = FindFirstObjectByType<InteractionUI>(FindObjectsInactive.Include);
    }

    void Update()
    {
        FindInteractable();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
        }
    }

    void FindInteractable()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);

        IInteractable nearestInteractable = null;
        float nearestDistance = interactionRange;

        foreach (var hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }

        if (nearestInteractable != currentInteractable)
        {
            currentInteractable = nearestInteractable;

            if (interactionUI != null)
            {
                if (currentInteractable != null)
                {
                    interactionUI.ShowInteraction(currentInteractable.GetInteractionText());
                }
                else
                {
                    interactionUI.HideInteraction();
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}