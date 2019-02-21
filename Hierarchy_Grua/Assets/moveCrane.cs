using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCrane : MonoBehaviour
{
    public float speed = 1.5f;
    public GameObject baseCrane;
    Rigidbody rb_base;
    public GameObject pluma;
    Rigidbody rb_pluma;

    public GameObject inicioPluma;
    public GameObject finalPluma;

    public GameObject eslabonPrefab;
    public List<GameObject> eslabones;
    public GameObject peso;
    

    // Use this for initialization
    void Start()
    {
        //Inicializacion variables
        rb_base = baseCrane.GetComponent<Rigidbody>();
        rb_pluma = pluma.GetComponent<Rigidbody>();
        GameObject eslabon = Instantiate(eslabonPrefab);
        eslabon.GetComponent<HingeJoint>().connectedBody = rb_pluma;       
        eslabon.GetComponent<Renderer>().material.color = new Color(1f, 0, 0);
        eslabones.Add(eslabon);
    }

    // Update is called once per frame
    void Update()
    {
        //Inputs de movimiento
        float horizontal = Input.GetAxis("Horizontal"); //A + D
        float vertical = Input.GetAxis("Vertical"); //W + S
        float rotation = Input.GetAxis("Rotation"); //Q + E

        //Calculo de rotacion y desplazamiento
        Vector3 movement = new Vector3(horizontal * speed * Time.deltaTime, 0, vertical * speed * Time.deltaTime);
        Quaternion deltaRotation = Quaternion.Euler(0, rotation * (speed*10) * Time.deltaTime, 0);

        //Movimiento y rotacion grua
        rb_base.MovePosition(rb_base.transform.position + movement);
        rb_base.MoveRotation(rb_base.transform.rotation * deltaRotation);

        //Rotacion pluma
        rb_pluma.MoveRotation(rb_pluma.transform.rotation * deltaRotation);

        //Movimiento pluma --> J + L
        if (Input.GetAxis("PlumaHorizontal") > 0){ 
            rb_pluma.position += (finalPluma.transform.position - rb_pluma.transform.position) * 0.01f;
            rb_pluma.transform.parent.position = rb_pluma.position;
        }
        else if (Input.GetAxis("PlumaHorizontal") < 0){ 
            rb_pluma.position += (inicioPluma.transform.position - rb_pluma.transform.position) * 0.01f;
            rb_pluma.transform.parent.position = rb_pluma.position;
        }
        else
            rb_pluma.MovePosition(rb_pluma.transform.parent.position + movement);

        //Movimiento cadena
        if (Input.GetMouseButtonDown(0)) //izq
            crearEslabon();
        if (Input.GetMouseButtonDown(1)) //der
            borrarEslabon();
        if (Input.GetMouseButtonDown(2)) //centro
            soltarPeso();

    }

    //Bajar cadena --> El ultimo eslabon creado funciona como enganche y es de color rojo.
    void crearEslabon(){
       if(eslabones.Count <= 10) { //LImite de 10 eslabones

            //El ultimo eslabon creado, se pinta de negro y te taggea como eslabon
            eslabones[eslabones.Count-1].GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f);
            eslabones[eslabones.Count - 1].tag = "eslabon";

            //Se crea uno nuevo, se coloca en el sitio deseado (bajo el ultimo eslabon de la cadena y con su misma rotacion)
            GameObject eslabon = Instantiate(eslabonPrefab);            
            eslabon.transform.rotation = eslabones[eslabones.Count - 1].transform.rotation;            
            eslabon.transform.position = new Vector3 (eslabones[eslabones.Count - 1].transform.position.x, 
                                        eslabones[eslabones.Count - 1].transform.position.y, 
                                        eslabones[eslabones.Count - 1].transform.position.z);            
            eslabon.transform.Translate(new Vector3(0, -1f, 0),  Space.Self);

            //Se crea un joint con el eslabon anterior
            eslabon.GetComponent<HingeJoint>().connectedBody = eslabones[eslabones.Count - 1].GetComponent<Rigidbody>();
            
            //Se añade a la cadena y al ser el ultimo se taggea como enganche y se pinta de rojo
            eslabones.Add(eslabon);
            eslabones[eslabones.Count - 1].GetComponent<Renderer>().material.color = new Color(1f, 0, 0);
            eslabones[eslabones.Count - 1].tag = "enganche";

            //Si la cadena ya llevaba un peso enganchado, se renueva el body del joint con el nuevo enganche
            if(peso != null){
                peso.GetComponent<HingeJoint>().connectedBody = eslabones[eslabones.Count - 1].GetComponent<Rigidbody>();
                peso.GetComponent<HingeJoint>().connectedAnchor = new Vector3(0, -0.5f, 0);
            }
        }        
    }

    //Subir cadena --> Siempre se borra el ultimo eslabon, que funcionaba como enganche
    void borrarEslabon(){               
        if (eslabones.Count > 1){//Siempre tiene que haber 1 eslabon como mínimo   
            
            //Se borra el elemento y se actualiza la lista
            Destroy(eslabones[eslabones.Count - 1]);
            eslabones.Remove(eslabones[eslabones.Count - 1]);

            //Se pinta el anterior de rojo y se taggea como enganche
            eslabones[eslabones.Count - 1].GetComponent<Renderer>().material.color = new Color(1f, 0, 0);
            eslabones[eslabones.Count - 1].tag = "enganche";

            //Si la cadena llevaba un peso, se actualiza el body del joint conectado con el nuevo enganche
            if (peso != null){
                peso.GetComponent<HingeJoint>().connectedBody = eslabones[eslabones.Count - 1].GetComponent<Rigidbody>();
                peso.GetComponent<HingeJoint>().connectedAnchor = new Vector3(0, -0.5f, 0);
            }
        }       
    }

    //Soltar peso --> Destruye el joint de union
    void soltarPeso(){
        if(peso != null){         
            Destroy(peso.GetComponent<HingeJoint>());
            peso = null;
        }
    }
}


