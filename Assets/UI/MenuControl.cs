using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject MainMenu;
    public void ToggleMenu()
    {
        if (MainMenu != null)
        {
                MainMenu.SetActive(false);
        }
    }
}
