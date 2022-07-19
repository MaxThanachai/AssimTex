using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assembler : MonoBehaviour
{
    public GameObject placeholderPrefab;

    LayerMask looseBlockMask;
    GridManager gridManager;

    bool isDragging;
    GameObject draggingBlock;
    GameObject placeholder;

    void Start()
    {
        InstantiateConfigs();
    }

    void InstantiateConfigs()
    {
        looseBlockMask = LayerMask.GetMask("LooseBlock");
        gridManager = gameObject.GetComponent<GridManager>();
    }

    void Update()
    {
        if (isDragging && !Input.GetMouseButton(0))
        {
            gridManager.NotifyDroppedLooseBlock(GetMouseWorldPosition(), draggingBlock);
            Destroy(placeholder);
            isDragging = false;
            placeholder = null;
            draggingBlock = null;
        }
        if (isDragging)
        {
            placeholder.transform.position = GetMouseWorldPosition();
            placeholder.transform.rotation = transform.rotation;
        }
        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = GetMouseWorldPosition();
            GameObject looseBlock = FindOneLooseBlockAt(clickPosition);
            if (looseBlock != null)
            {
                InstantiatePlaceHolder(clickPosition);
                draggingBlock = looseBlock;
                isDragging = true;
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickPosition.z = 0;
        return clickPosition;
    }

    GameObject FindOneLooseBlockAt(Vector3 clickPosition)
    {
        Collider2D collider = Physics2D.OverlapCircle(new Vector2(clickPosition.x, clickPosition.y), 1, looseBlockMask);
        if (collider != null)
        {
            return collider.gameObject;
        }
        return null;
    }

    GameObject InstantiatePlaceHolder(Vector3 clickPosition)
    {
        placeholder = Instantiate(placeholderPrefab, clickPosition, transform.rotation, transform);
        // TODO: Make the placeholder take sprite from the block
        return placeholder;
    }
}
