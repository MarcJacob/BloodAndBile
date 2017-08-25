using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * <summary> Gestion de la console de développement. </summary>
 * SINGLETON
 */ 
public class Console : MonoBehaviour
{
    //STATIC
    static Console Instance;
    const int MAX_LINE_MEMORY = 200;

    static ConsoleLine[] Lines = new ConsoleLine[MAX_LINE_MEMORY]; // Tableau contenant une ligne par case. La taille du tableau et donc le nombre maximal de lignes gardées en mémoire
                           // correspond à la constante "MAX_LINE_MEMORY".
    //

    // Affichage
    Canvas TextArea;
    InputField CommandInput;
    bool Initialised = false;
    int NumberOfLines = 0;
    int HeightPerLine = 18;
    Text[] TextLines;
    bool Enabled = true;

    // Données

    

    public GameObject TextLinePrefab;

    /**
     * <summary> Active la console (en affichant le canvas. Techniquement, la console est toujours "activée", mais la plupart des
     * fonctionnalités de celles ci sont désactivées quand Enabled est égal à false.</summary>
     */
    void Enable()
    {
        Instance.Enabled = true;
        GetComponent<Canvas>().enabled = true;
        CommandInput.Select();
        CommandInput.ActivateInputField();
    }

    void Disable()
    {
        Instance.Enabled = false;
        GetComponent<Canvas>().enabled = false;
        CommandInput.DeactivateInputField();
    }

    /**
     * <summary> Défini le contenu de la case 0 du tableau lines, et décale toutes les autres cases d'un cran. La dernière case est
     * donc supprimée. </summary>
     */
    public void WriteLine(string line, Color col)
    {
        // Décalage de toutes les lignes d'un cran.
        for (int i = MAX_LINE_MEMORY - 1; i > 0; i--)
        {
            Lines[i] = Lines[i - 1];
        }

        Lines[0] = new ConsoleLine(line, col);
    }


    void Start()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            TextArea = GetComponent<Canvas>();
            CommandInput = transform.Find("InputField").GetComponent<InputField>();
            if (TextArea == null || CommandInput == null)
            {
                Debugger.Log("Console mal initialisé : pas de Canvas ou d'InputField disponible !");

            }
            else
            {
                Initialised = true;
                TextLines = new Text[20];
                // On assigne la commande "Log" à l'affichage de texte sur la console.
                InputManager.AddHandler("Log", Debugger.Log);
                InputManager.AddHandler("Help", DisplayHelp);
                DontDestroyOnLoad(gameObject);

                Enable(); // Par défaut la console est désactivée.
            }
        }
    }

    /**
     * <summary> Mise à jour du nombre de lignes à afficher. </summary>
     */ 
    void Update()
    {
        if (Enabled)
        {
            UpdateNumberOfLines();
            UpdateLineRendering();
        }
        CheckInput();
    }

    /**
     * <summary> Met à jour le nombre de lignes affichées à l'écran. </summary>
     */ 
    void UpdateNumberOfLines()
    {
        int newNumberOfLines = (int)((TextArea.GetComponent<RectTransform>().rect.height - CommandInput.GetComponent<RectTransform>().rect.height) / HeightPerLine);
        if (newNumberOfLines != NumberOfLines)
        {
            // Le nombre de lignes a changé !
            // On change la taille du tableau TextLines et on replace chaque Text au bon endroit à l'écran.
            if (TextLines.Length > 0)
            foreach(Text t in TextLines)
            {
                if(t != null)
                Destroy(t.gameObject);
            }
            TextLines = new Text[newNumberOfLines];
            NumberOfLines = newNumberOfLines;

            // Créer les GameObjects qui contiennent les Text.
            int lineID = 0;
            while (lineID < TextLines.Length)
            {
                TextLines[lineID] = (Instantiate(TextLinePrefab, transform) as GameObject).GetComponent<Text>();
                lineID++;
            }

            // Maintenant, il faut ajuster la position et la hauteur de chaque ligne.
            RectTransform CanvasTransform = GetComponent<RectTransform>();
            for (lineID = 0; lineID < TextLines.Length; lineID++)
            {
                // En fonction de lineID, on place la ligne à une certaine hauteur.
                float yPosition = CommandInput.GetComponent<RectTransform>().rect.height
                    + HeightPerLine * lineID;
                TextLines[lineID].GetComponent<RectTransform>().anchorMin = new Vector2(0, yPosition / CanvasTransform.rect.height);
                TextLines[lineID].GetComponent<RectTransform>().anchorMax = new Vector2(1, (yPosition+HeightPerLine) / CanvasTransform.rect.height);
                TextLines[lineID].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CanvasTransform.rect.width);
                TextLines[lineID].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, HeightPerLine);
            }
        }
    }

    /**
     * <summary> Assigne ou ré-assigne le texte à afficher pour chaque ligne visible à l'écran. </summary>
     */ 
    void UpdateLineRendering()
    {
        for(int lineID = 0; lineID < TextLines.Length; lineID++)
        {
            if (TextLines[lineID] != null)
            TextLines[lineID].text = Lines[lineID].Text;
            TextLines[lineID].color = Lines[lineID].TextColor;
        }
    }

    /**
     * <summary> Vérifie s'il faut envoyer ce que l'utilisateur a écrit dans l'InputField vers le système de commande. </summary>
     */ 
    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            if (Enabled)
                Disable();
            else
                Enable();
        }
        if (Enabled && Input.GetKeyDown(KeyCode.Return))
        {
            // On obtient le texte tapé dans l'InputField puis on sépare le nom de la commande des paramètres s'il y en a.
            string rawText = CommandInput.text;
            if (rawText != "")
            {
                Debugger.Log("---- " + rawText);
                DecomposeAndSendCommand(rawText);

                CommandInput.text = "";

                // Quand on appuie sur "Entrée", Unity (bizarrement) décide d'enlever le focus de l'InputField même si on vient d'y taper
                // des choses. Cela n'est pas pratique pour une console de développement, donc après avoir envoyé une commande avec succès,
                // on force Unity à conserver le focus sur l'InputField.
                CommandInput.Select();
                CommandInput.ActivateInputField();
            }


        }
    }

    /**
     * <summary> Décompose une commande en un nom de commande et un tableau de paramètres, puis les envoi au système de commande. </summary>
     */ 
    void DecomposeAndSendCommand(string rawText)
    {
        string[] commandParts = rawText.Split(' ');
        string commandName = commandParts[0];
        if (commandParts.Length > 1)
        {
            List<object> parameters = new List<object>();
            for(int elementID = 1; elementID < commandParts.Length; elementID++)
            {
                parameters.Add(commandParts[elementID]);
            }

            InputManager.SendCommand(commandName, parameters.ToArray());
        }
        else
        {
            InputManager.SendCommand(commandName);
        }
    }


    Color helpColor = Color.cyan;
    /**
     * <summary> Exécutée lorsqu'un utilisateur de la console tape "Help". </summary>
     */
    public void DisplayHelp(object[] parameters)
    {

        WriteLine("'Help' - Affiche cette liste.", helpColor);
        WriteLine("'Exit' - Quitte l'application.", helpColor);
        WriteLine("'Log <message>' - Affiche un message.", helpColor);
    }
}