using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using TMPro;


public class SaveSystem : MonoBehaviour
{
    // 存档文件的路径
    public string saveFilePath = "/Saves/";
    public Button button; // 按钮变量
    public GameObject panel; // 面板变量
    
    public string saveFileName = "defaultSave.json"; // 默认存档文件名
    public Text text1; // 第一个 Text 组件
    public Text text2; // 第二个 Text 组件
    public Text text3; // 第三个 Text 组件
    public string[] saveFiles;
    private bool isPanelActive = false; // 记录面板当前是否激活的状态
    public ButtonClickHandler buttonClickHandler;
    public PlayerAttributesclass playerattributes;
    public TextMeshProUGUI statusText;



    public void savaall()
    {
        buttonClickHandler.SaveUnlockedNodes();
        playerattributes.SavePlayerAttributes();
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData("CurrentScene", currentSceneName);
    }

    private void Start()
    {
        // 在 Start 函数中为按钮添加点击事件监听器
        button.onClick.AddListener(TogglePanel);

        // 检查存储位置是否有存储文件
        CheckSaveFiles();
    }

    public void Rename(string oldFilePath, string newFileName)
    {
        // 获取旧文件的文件名
        Debug.Log(oldFilePath);
        Debug.Log(newFileName);
        try
        {
            // 如果旧文件存在，则将其重命名为新文件名
            if (File.Exists(oldFilePath))
            {
                Debug.Log(oldFilePath);
                Debug.Log(newFileName);
                File.Move(oldFilePath, newFileName);
                Debug.Log("File renamed from " + oldFilePath + " to " + newFileName);
            }
            else
            {
                Debug.LogWarning("File does not exist: " + oldFilePath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error renaming file: " + e.Message);
        }
    }

    // 清除指定存档文件的内容
    public void ClearSaveFile(string fileName)
    {
        // 构建存档文件的完整路径
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string newFilePath = Path.Combine(Application.persistentDataPath, "empty");

        try
        {
            // 如果存档文件存在，则清空其内容
            if (File.Exists(filePath))
            {
                // 清空文件内容
                File.WriteAllText(filePath, string.Empty);
                Debug.Log("Save file cleared: " + fileName);

                // 重命名文件
                File.Move(filePath, newFilePath);
                Debug.Log("Save file renamed to: empty");
            }
            else
            {
                Debug.LogWarning("Save file does not exist: " + fileName);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error clearing save file: " + e.Message);
        }
    }


    // 切换面板的显示和隐藏状态
    public void TogglePanel()
    {
        isPanelActive = !isPanelActive; // 切换状态
        panel.SetActive(isPanelActive); // 根据新状态设置面板的激活状态
    }

    

    // 检查存储位置是否有存储文件
    private void CheckSaveFiles()
    {
        // 构建存储位置的完整路径
        string saveFolderPath = Application.persistentDataPath;

        // 获取存储位置中的所有文件
        saveFiles = Directory.GetFiles(saveFolderPath, "*.txt");
        if (saveFiles.Length < 3)
        {
            // 创建足够数量的空文件
            for (int i = 0; i < 3 - saveFiles.Length; i++)
            {
                File.WriteAllText(Path.Combine(saveFolderPath, "emptyFile_" + i + ".txt"), "");
            }

            // 重新获取存储位置中的所有文件
            saveFiles = Directory.GetFiles(saveFolderPath, "*.txt");
        }
        // 检查存储位置中是否有 .txt 类型的文件
        if (saveFiles.Length > 0)
        {
            // 将文件名分别赋值给三个 Text 组件
            text1.text = Path.GetFileNameWithoutExtension(saveFiles[0]);
            text2.text = Path.GetFileNameWithoutExtension(saveFiles.Length > 1 ? saveFiles[1] : "");
            text3.text = Path.GetFileNameWithoutExtension(saveFiles.Length > 2 ? saveFiles[2] : "");
        }
        else
        {
            Debug.Log("No .txt files found in save folder.");
            button.image.color = Color.gray;
            // 将按钮变为不可点击的状态
            button.interactable = false;
        }
    }





    public void SaveData(string key, string data)
    {
        string savafilename = PlayerPrefs.GetString("LoadedFileName");
        // 读取文件中已有的信息
        string existingData = LoadData();

        // 创建一个二进制格式化程序
        BinaryFormatter formatter = new BinaryFormatter();

        // 创建一个内存流来保存二进制数据
        using (MemoryStream stream = new MemoryStream())
        {
            // 将数据序列化为二进制格式并写入内存流中
            formatter.Serialize(stream, data);

            // 将内存流的内容转换为字节数组
            byte[] binaryData = stream.ToArray();

            // 将字节数组转换为 Base64 编码的字符串，以便保存到文件中
            string encodedData = Convert.ToBase64String(binaryData);

            // 检查是否已存在相同的键值
            int keyIndex = existingData.IndexOf(key + ":");
            if (keyIndex != -1)
            {
                // 已存在相同的键值，覆盖原有的数据
                int endIndex = existingData.IndexOf("\n", keyIndex);
                string newData = existingData.Substring(0, keyIndex) + key + ":" + encodedData + "\n" + existingData.Substring(endIndex + 1);
                existingData = newData;
            }
            else
            {
                // 不存在相同的键值，添加新的键值对
                existingData += key + ":" + encodedData + "\n";
            }
        }

        // 将新的信息写入文件
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
        // 读取文件中的所有信息
        string fileContent = LoadData();
        string savafilename = PlayerPrefs.GetString("LoadedFileName");

        // 在文件中查找指定键值的信息
        int startIndex = fileContent.IndexOf(key + ":");
        if (startIndex != -1)
        {
            // 找到指定键值的信息，截取并返回
            int endIndex = fileContent.IndexOf("\n", startIndex);
            if (endIndex != -1)
            {
                string encodedData = fileContent.Substring(startIndex + key.Length + 1, endIndex - startIndex - key.Length - 1);

                // 将 Base64 编码的字符串解码为字节数组
                byte[] binaryData = Convert.FromBase64String(encodedData);

                // 创建一个二进制格式化程序
                BinaryFormatter formatter = new BinaryFormatter();

                // 创建一个内存流来从字节数组中读取数据
                using (MemoryStream stream = new MemoryStream(binaryData))
                {
                    // 将字节数组反序列化为原始数据类型
                    string data = (string)formatter.Deserialize(stream);
                    return data;
                }
            }
        }

        // 没有找到指定键值的信息
        return null;
    }


    private string LoadData()
    {
        // 从文件中读取所有信息
        string savafilename = PlayerPrefs.GetString("LoadedFileName");
        Debug.Log(savafilename + "\n");
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


    public void NewGame()
    {
        for (int i = 0; i <saveFiles.Length; i++)
        {
            string fileName = saveFiles[i];
            PlayerPrefs.SetString("LoadedFileName", fileName);
            PlayerPrefs.Save();
            string existingData = LoadData();


            // 检查文件是否为空
            if (string.IsNullOrEmpty(existingData))
            {
                DateTime currentTime = DateTime.Now;
                string savedTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
                savedTime = savedTime.Replace(" ", "-");
                savedTime = savedTime.Replace(":", "-");
                string filePath = Path.Combine(Application.persistentDataPath, savedTime + ".txt");
                Rename(fileName, filePath);
                PlayerPrefs.SetString("LoadedFileName", filePath);
                PlayerPrefs.Save();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {

                if(i==2)
                {
                    UpdateStatusText("please delete", true);
                }

                // 将文件名写入 PlayerPrefs

            }
        }
    }



    public void LoadGame(int fileIndex)
    {


        string fileName = saveFiles[fileIndex];
        PlayerPrefs.SetString("LoadedFileName", fileName);
        PlayerPrefs.Save();
        // 读取文件中已有的信息
        string existingData = LoadData();


        // 检查文件是否为空
        if (string.IsNullOrEmpty(existingData))
        {
            // 加载名为 "Load" 的场景
            DateTime currentTime = DateTime.Now;
            string savedTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            savedTime = savedTime.Replace(" ","-");
            savedTime = savedTime.Replace(":", "-");
            string filePath = Path.Combine(Application.persistentDataPath, savedTime+".txt");
            Rename(fileName, filePath);
            PlayerPrefs.SetString("LoadedFileName", filePath);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            // 将文件名写入 PlayerPrefs



        }
        else
        {
            Debug.Log(string.IsNullOrEmpty(existingData));
            PlayerPrefs.SetString("LoadedFileName", fileName);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            // 将文件名写入 PlayerPrefs
            
        }
    }

    public void UpdateStatusText(string message, bool isActive)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.gameObject.SetActive(isActive);
            Invoke(nameof(HideStatusText), 3f); // 3秒后隐藏文本组件
        }
    }

    // 在指定延迟后隐藏文本组件的方法
    private void HideStatusText()
    {
        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
        }
    }


}
