using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    private Renderer changecolorTo;

    private Color OGColor;


    [SerializeField] public TextMeshProUGUI UItext;

    // Start is called before the first frame update
    void Start()
    {
        changecolorTo = GetComponent<Renderer>();
        if (changecolorTo != null)
        {
            OGColor = changecolorTo.material.color;
        }
        else
        {
            Debug.LogError("material is missing....");
        }
        DisplayTheText(false);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {

                changecolorTo.material.color = Color.gray;
                DisplayTheText(true);
            }
            else
            {

                changecolorTo.material.color = OGColor;
                DisplayTheText(false);
            }

        }
        else
        {
            changecolorTo.material.color = OGColor;
            DisplayTheText(false);
        }
    }

    public string GetTheText()
    {
        return UItext.text;
    }

    private void DisplayTheText(bool enableTheText)
    {
        if (UItext != null)
        {
            UItext.gameObject.SetActive(enableTheText);
        }
        else { Console.WriteLine("gride text is empty"); }
    }
}
