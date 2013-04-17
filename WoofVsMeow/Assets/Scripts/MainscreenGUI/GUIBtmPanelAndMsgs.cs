using UnityEngine;
using System.Collections;

public class GUIBtmPanelAndMsgs : MonoBehaviour
{
    private float m_optimalHeight = 900.0f; //height of screen in free aspect ratio
    private float m_optimalWidth = 1600.0f; //width of screen in free aspect ratio

    public Texture2D m_moneyTex;
    public Texture2D m_commanderTex;

    public GUIStyle m_boxStyle;//For the boxes carrying textures
	public GUIStyle m_buttonStyle;
    public GUIStyle m_labelStyle;//For the labels in the bottom panel
    public GUIStyle m_messageStyle;//For the messages that appear at the top of the screen. They need a larger font

    private float m_labelWidth = 200.0f; //Width of the labels in bottom panel
    private float m_labelHeight = 40.0f; //Height of the labels in bottom panel

    private float m_boxWidth = 75.0f; //Width of the texture boxes in bottom panel
    private float m_boxHeight = 75.0f;//Height of the texture boxes in bottom panel

    private float m_menuBtnWidth = 300.0f; //Width of the menu button
    private float m_menuBtnHeight = 150.0f;//Height of the menu button

    private string m_playerName;
    private double m_money;

    private bool m_gameOver;
    private string m_winner;
    private bool m_connectionError;
    private string m_errorMsg;
    private bool m_waitingForTurn;

    // Use this for initialization
    public void Initialise(int money)
    {
        m_playerName = PlayerPrefs.GetString("playername");
        m_money = money;

        m_gameOver = false;
        m_connectionError = false;
        m_waitingForTurn = false;
    }

    public void GameWonBy(string name)
    {
        m_winner = name;
        m_gameOver = true;
    }

    public void ConnectionError(string errorMsg)
    {
        m_errorMsg = errorMsg;
        m_connectionError = true;
    }

    public void SetMoney(int money)
    {
        m_money = money;
    }

    public void OnMyTurn()
    {
        m_waitingForTurn = false;
    }
    public void OnOthersTurn()
    {
        m_waitingForTurn = true;
    }

    void OnGUI()
    {
        float ratio = Screen.width / m_optimalWidth;

        //Bottom panel
        GUI.Box(new Rect(0, Screen.height - 110.0f * ratio, Screen.width, 110.0f * ratio), "");

        //Name Label
        GUI.Label(new Rect(150.0f * ratio, Screen.height - 75.0f * ratio, m_labelWidth * ratio, m_labelHeight * ratio),
            "Name : " + m_playerName, m_labelStyle);
        //Money texture
        GUI.Box(new Rect(Screen.width - (m_labelWidth + m_boxWidth + 300.0f) * ratio, Screen.height - 100.0f * ratio, m_boxWidth * ratio, m_boxHeight * ratio),
            m_moneyTex, m_boxStyle);
        //Money label
        GUI.Label(new Rect(Screen.width - (m_labelWidth + 200.0f) * ratio, Screen.height - 75.0f * ratio, m_labelWidth * ratio, m_labelHeight * ratio),
            "m_money : $" + m_money.ToString(), m_labelStyle);

        if (m_gameOver)
        {
            GUI.Label(new Rect(0, 20 * ratio, Screen.width, m_labelHeight * ratio),
            m_winner + " has won!", m_messageStyle);

            //Centered menu button
            if (GUI.Button(new Rect(Screen.width / 2 - (m_menuBtnWidth / 2) * ratio, Screen.height / 2 - (m_menuBtnHeight / 2) * ratio,
                m_menuBtnWidth * ratio, m_menuBtnHeight * ratio), "Return to menu", m_buttonStyle))
            {
                //close connections
                Network.Disconnect();
                if (Network.isServer)
                {
                    MasterServer.UnregisterHost();
                }
                Application.LoadLevel("NetworkMenu");
            }
        }
        else if (m_connectionError)
        {
            GUI.Label(new Rect(0, 20 * ratio, Screen.width, m_labelHeight * ratio),
            m_errorMsg, m_messageStyle);
            //Centered menu button
            if (GUI.Button(new Rect(Screen.width / 2 - (m_menuBtnWidth / 2) * ratio, Screen.height / 2 - (m_menuBtnHeight / 2) * ratio,
                m_menuBtnWidth * ratio, m_menuBtnHeight * ratio), "Return to menu", m_buttonStyle))
            {
                Network.Disconnect();
                if (Network.isServer)
                {
                    MasterServer.UnregisterHost();
                }
                Application.LoadLevel("NetworkMenu");
            }
        }
        else if (m_waitingForTurn)
        {
            GUI.Label(new Rect(0, 20 * ratio, Screen.width, m_labelHeight * ratio),
            "Waiting for opponent's move...", m_messageStyle);
        }
    }
}
