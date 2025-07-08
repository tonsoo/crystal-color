using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EstatisticasManager : MonoBehaviour
{
    public GameManager game_manager;

    public GameObject placa_prefab;
    public List<DialogTrigger> placas = new List<DialogTrigger>();
    public DialogTrigger placa_cristais_primeira;
    public DialogTrigger placa_cristais_erros;
    public DialogManager dialog_manager;

    public AudioClip clip;

    public iteract_object_data[] data;

    public int firstTry = 0, errors = 0, tries = 0;

    public string[] random_dialog = new string[]
    {
        "Isso não é incrivel??",
        "Uau, isso é um número grande.",
        "Joga muito!!",
        "Parabéns!!!",
        "Talvez você consiga um número mais alto na próxima..."
    };

    public void setPlacas()
    {
        CrystalManager[] gm = FindObjectsOfType<CrystalManager>();

        for (int i = 0; i < gm.Length; i++)
        {
            firstTry += gm[i].firstTry;
            errors += gm[i].errors;
            tries += gm[i].tries;
        }

        placa_cristais_primeira.dialogs[0].dialogs[0].dialog_text = "Você acertou " + ((float)((float)firstTry / (float)gm.Length) * 100).ToString("F2") + "% das sequencias sem errar!";
        placa_cristais_erros.dialogs[0].dialogs[0].dialog_text = "Infelizmente você errou " + ((float)((float)errors / (float)tries) * 100).ToString("F2") + "% das suas tentativas de ativar o cristal correto...";
        placa_cristais_erros.dialogs[0].dialogs[1].dialog_text = "Mas por outro lado escolheu os cristais corretos " + ((float)((float)(tries - errors) / (float)tries) * 100).ToString("F2") + "% das vezes!";

        data = game_manager.estatisticas;

        List<int> escolhidos = new List<int>();

        for (int i = 0; i < data.Length; i++)
        {
            escolhidos.Add(i);
        }

        Debug.Log(escolhidos.Count);

        for (int i = 0; i < placas.Count; i++)
        {
            int j = Random.Range(0, escolhidos.Count);

            string text = "Você interagiu " + data[j].times_interacted + " vezes com " + data[j].object_name + "!";

            placas[i].dialogs[0].dialogs[0].dialog_text = text;
            text = "Com isso você interagiu com " + ((float)((float)data[j].times_interacted / (float)data[j].object_amount) * 100).ToString("F2") + "% dos objetos desse tipo no mapa.";
            placas[i].dialogs[0].dialogs[1].dialog_text = text;
            text = random_dialog[Random.Range(0, random_dialog.Length)];
            placas[i].dialogs[0].dialogs[2].dialog_text = text;

            escolhidos.Remove(j);
        }

        for (int i = 0; i < placas.Count; i++)
        {
            if (placas[i].dialogs[0].dialogs[0].dialog_text == "")
            {
                Destroy(placas[i].gameObject, .2f);
            }
        }
    }
}
