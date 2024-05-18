using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class ButtonClickHandler : MonoBehaviour
{
    public GameObject canvasPanel;
    public GameObject kejiPanel;
    public Text descriptionText; // 添加对 Text 组件的引用
    public List<Image> iconImages;
    public List<bool> unlockedNodes;
    public List<bool> ablelockedNodes;
    private int left;
    private int right;
    public AudioSource audioSource;
    public AudioClip yourAudioClip;
    public LocalizationManager localizationManagerPrefab;




    public void deletetanchaung()
    {
        canvasPanel.SetActive(false);
    }
    //技能解释
    public void SetSkillDescription(string skillIndex)
    {
        // 检查 localizationManagerPrefab 是否为空
        if (localizationManagerPrefab == null)
        {
            Debug.LogError("Localization Manager Prefab is not assigned!");
            return;
        }

        // 获取对应的文本
        string skillDescription = localizationManagerPrefab.GetLocalizedValue(skillIndex);

        // 检查是否找到了对应的文本
        if (string.IsNullOrEmpty(skillDescription))
        {
            Debug.LogWarning("Skill description not found for index: " + skillIndex);
            return;
        }

        // 将文本赋值给文本组件
        if (descriptionText != null)
        {
            descriptionText.text = skillDescription;
        }
        else
        {
            Debug.LogError("Text component not found! Make sure to assign the Description Text in the inspector.");
        }

        // 唤醒 canvas 面板
        if (canvasPanel != null)
        {
            canvasPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Canvas panel reference not found! Make sure to assign the Canvas Panel in the inspector.");
        }
    }




    public void SaveUnlockedNodes()
    {
        string unlockedCsv = string.Join(",", unlockedNodes.Select(node => node ? "1" : "0").ToArray());
        string ablelockedCsv = string.Join(",", ablelockedNodes.Select(node => node ? "1" : "0").ToArray());

        SaveData("unlockedNodesCSV", unlockedCsv);
        SaveData("ablelockedNodesCSV", ablelockedCsv);
    }

    private void LoadUnlockedNodes()
    {
        string unlockedCsv = LoadData("unlockedNodesCSV");
        Debug.Log(unlockedCsv);
        string ablelockedCsv = LoadData("ablelockedNodesCSV");
        Debug.Log(ablelockedCsv);
        if (string.IsNullOrEmpty(unlockedCsv) || string.IsNullOrEmpty(ablelockedCsv))
        {
            // 如果CSV字符串为空，则根据iconImages列表的长度初始化unlockedNodes和ablelockedNodes列表  
            unlockedNodes = new List<bool>();
            ablelockedNodes = new List<bool>();
            foreach (Image iconImage in iconImages)
            {
                unlockedNodes.Add(false); // 假设所有图标默认都是未解锁的
                ablelockedNodes.Add(false); // 假设所有节点默认都不可解锁
            }

        }
        else
        {
            // 如果CSV字符串不为空，则解析为bool列表  
            unlockedNodes = new List<bool>();
            ablelockedNodes = new List<bool>();
            foreach (string value in unlockedCsv.Split(','))
            {
                bool nodeState = value == "1"; // 假设"1"代表true，"0"代表false  
                unlockedNodes.Add(nodeState);
            }
            foreach (string value in ablelockedCsv.Split(','))
            {
                bool nodeState = value == "1"; // 假设"1"代表true，"0"代表false  
                ablelockedNodes.Add(nodeState);
            }
        }
    }

    private void Start()
    {
        LoadUnlockedNodes();
        unlockedNodes[0] = true;
        ablelockedNodes[1] = true;
        ablelockedNodes[2] = true;
        ablelockedNodes[3] = true;
        ablelockedNodes[4] = true;
        UpdateIconColors();
    }

    public void OnButtonClick()
    {
        kejiPanel.SetActive(!kejiPanel.activeSelf);
    }




    public void UnlockNode(int nodeIndex)
    {
        if (ablelockedNodes[nodeIndex] && !unlockedNodes[nodeIndex])
        {
            audioSource.clip = yourAudioClip;
            audioSource.Play();
            unlockedNodes[nodeIndex] = true;
            ablelockedNodes[left] = true;
            ablelockedNodes[right] = true;

            SaveUnlockedNodes();
            UpdateIconColors();
        }
    }

    public void leftUnlockNode(int leftIndex)
    {
        left = leftIndex;
    }

    public void rightUnlockNode(int rightIndex)
    {
        right = rightIndex;
    }

    private void UpdateIconColors()
    {
        for (int i = 0; i < iconImages.Count; i++)
        {
            if (unlockedNodes[i])
            {
                iconImages[i].color = Color.white;
            }
            else
            {
                if (ablelockedNodes[i])
                {
                    iconImages[i].color = Color.gray;
                }
                else
                {
                    iconImages[i].color = Color.black;
                }
            }
        }
    }

    public bool IsNodeClickable(int nodeIndex)
    {
        return ablelockedNodes[nodeIndex];
    }

    public void SaveData(string key, string data)
    {
        string savafilename = PlayerPrefs.GetString("LoadedFileName");
        string existingData = LoadData();
        Debug.Log(existingData);

        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, data);
            byte[] binaryData = stream.ToArray();
            string encodedData = Convert.ToBase64String(binaryData);

            int keyIndex = existingData.IndexOf(key + ":");
            if (keyIndex != -1)
            {
                int endIndex = existingData.IndexOf("\n", keyIndex);
                string newData = existingData.Substring(0, keyIndex) + key + ":" + encodedData + "\n" + existingData.Substring(endIndex + 1);
                existingData = newData;
            }
            else
            {
                existingData += key + ":" + encodedData + "\n";
            }
        }

        try
        {
            File.WriteAllText(savafilename, existingData);
            Debug.Log("Data saved to file: " + savafilename);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving data to file: " + e.Message);
        }
    }

    public string LoadData(string key)
    {
        string fileContent = LoadData();
        string savafilename = PlayerPrefs.GetString("LoadedFileName");

        int startIndex = fileContent.IndexOf(key + ":");
        if (startIndex != -1)
        {
            int endIndex = fileContent.IndexOf("\n", startIndex);
            if (endIndex != -1)
            {
                string encodedData = fileContent.Substring(startIndex + key.Length + 1, endIndex - startIndex - key.Length - 1);
                byte[] binaryData = Convert.FromBase64String(encodedData);

                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(binaryData))
                {
                    string data = (string)formatter.Deserialize(stream);
                    return data;
                }
            }
        }

        return null;
    }

    private string LoadData()
    {
        string savafilename = PlayerPrefs.GetString("LoadedFileName");
        try
        {
            if (File.Exists(savafilename))
            {
                return File.ReadAllText(savafilename);
            }
            else
            {
                Debug.LogWarning("Data file does not exist: " + savafilename);
                return "";
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading data from file: " + e.Message);
            return "";
        }
    }
}
