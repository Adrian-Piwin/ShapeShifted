﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

[System.Serializable]
public struct StoreItem 
{
    public Sprite sprite;
    public Color color;
    public int price;
}

public class StoreManagement : MonoBehaviour
{
    [Header("Store Items")]
    [SerializeField] private int storePageItemCount;
    [SerializeField] private List<StoreItem> storeItems;

    [Header("Store Button References")]
    [SerializeField] private List<Transform> storeButtons;
    [SerializeField] private List<Image> storeSprites;
    [SerializeField] private List<TextMeshProUGUI> storePrices;

    [SerializeField] private TextMeshProUGUI leftNavBtn;
    [SerializeField] private TextMeshProUGUI rightNavBtn;

    [Header("Player References")]
    [SerializeField] private SpriteRenderer player;
    [SerializeField] private SpriteRenderer playerDrag;

    [Header("Other References")]
    [SerializeField] private GameManagement gameManagement;
    [SerializeField] private TextMeshProUGUI coinText;

    private int coins = -1;
    private int currentIndex;
    private List<bool> isBoughtList;

    void Start() {
        // Set default skin as bought
        PlayerPrefs.SetInt("Bought 0", 1);

        // Update store with what is bought
        isBoughtList = new List<bool>(new bool[storeItems.Count]);
        for (int i = 0; i < storeItems.Count; i++) {
            isBoughtList[i] = PlayerPrefs.GetInt("Bought " + i, 0) == 0 ? false : true;
        }

        // Get current store items
        currentIndex = 0;

        SetupStore();
        UpdateBought();
        UpdateSelected();
        UpdateStoreNav();

        ChangePlayerSprite(PlayerPrefs.GetInt("Current Selection", 0));
    }

    public void MenuEnabled() 
    {
        // Call when menu is enabled
        coins = gameManagement.coins;
        coinText.text = "COINS: " + coins;
    }

    // =============== Update Btns ===============

    private void SetupStore() 
    {
        // Display items for store page

        // Setup sprites/colors
        int i = 0;
        foreach (Image img in storeSprites) 
        {
            img.sprite = storeItems[currentIndex + i].sprite;
            img.color = storeItems[currentIndex + i].color;
            i++;
        }

        // Setup prices
        i = 0;
        foreach (TextMeshProUGUI storeText in storePrices) 
        {
            storeText.text = storeItems[currentIndex + i].price + " COINS";
            i++;
        }
    }

    private void UpdateBought()
    {
        // Display if skin is already bought 

        int i = 0;
        foreach (Transform btn in storeButtons)
        {
            if (isBoughtList[currentIndex + i])
            {
                btn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "SELECT";
                storePrices[i].text = "";
            }
            else 
            {
                btn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "BUY";
                storePrices[i].text = storeItems[currentIndex + i].price + " COINS";
            }

            i++;
        }
    }

    private void UpdateSelected() {
        // Display if skin is selected

        int select = PlayerPrefs.GetInt("Current Selection", 0);

        foreach (Transform btn in storeButtons) {
            btn.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        }

        // Check if page has selected item
        if (select >= currentIndex && select <= currentIndex + storePageItemCount) 
        {
            // Display currently selected item by greying out button
            storeButtons[select - currentIndex].GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(70, 70, 70, 255);
        }
    }

    private void UpdateStoreNav() 
    {
        // Show arrows if able to go to the next page

        // Right btn
        if (storeItems.Count <= currentIndex + storePageItemCount)
        {
            // Cannot go to next page
            Color color = rightNavBtn.color;
            color.a = 0.3f;
            rightNavBtn.color = color;
        }
        else 
        {
            Color color = rightNavBtn.color;
            color.a = 1f;
            rightNavBtn.color = color;
        }

        // Left btn
        if (currentIndex - storePageItemCount < 0)
        {
            // Cannot go to previous page
            Color color = leftNavBtn.color;
            color.a = 0.3f;
            leftNavBtn.color = color;
        }
        else 
        {
            Color color = leftNavBtn.color;
            color.a = 1f;
            leftNavBtn.color = color;
        }
    }

    // =============== Player ===============

    private void ChangePlayerSprite(int select) 
    {
        // Update player sprites / colors

        // Player update
        player.sprite = storeItems[select].sprite;
        player.color = storeItems[select].color;

        // Player drag update
        playerDrag.sprite = storeItems[select].sprite;
        // Keep alpha from drag, then update new color
        Color dragColor = playerDrag.color;
        Color newDragColor = storeItems[select].color;
        newDragColor.a = dragColor.a;
        playerDrag.color = newDragColor;
    }

    // =============== On Btn Press ===============

    public void SelectItem(int select)
    {
        // Do on button press

        // Buy item
        if (!isBoughtList[currentIndex + select] && coins >= storeItems[currentIndex + select].price)
        {
            PlayerPrefs.SetInt("Bought " + (currentIndex + select), 1);
            isBoughtList[currentIndex + select] = true;
            
            coins -= storeItems[currentIndex + select].price;
            gameManagement.coins = coins;
            coinText.text = "COINS: " + coins;

            PlayerPrefs.SetInt("Current Selection", currentIndex + select);
            UpdateSelected();
            UpdateBought();
            ChangePlayerSprite(currentIndex + select);
        }
        // If item is already bought and not selected, select it
        else if (isBoughtList[currentIndex + select] && currentIndex + select != PlayerPrefs.GetInt("Current Selection"))
        {
            PlayerPrefs.SetInt("Current Selection", currentIndex + select);
            UpdateSelected();
            ChangePlayerSprite(currentIndex + select);
        }

    }
    public void OnLeftBtnPress()
    {
        // Left btn
        if (currentIndex - storePageItemCount >= 0)
        {
            // Go to prev page
            currentIndex -= storePageItemCount;
            SetupStore();
            UpdateBought();
            UpdateSelected();
            UpdateStoreNav();
        }
    }

    public void OnRightBtnPress()
    {
        // Right btn
        if (storeItems.Count > currentIndex + storePageItemCount)
        {
            // Go to next page
            currentIndex += storePageItemCount;
            SetupStore();
            UpdateBought();
            UpdateSelected();
            UpdateStoreNav();
        }
    }

    
}
