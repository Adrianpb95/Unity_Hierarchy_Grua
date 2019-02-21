using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeScript : MonoBehaviour {
 
    //Gestiona la colision de todos los cubos --> Cubos de colores y Pozos de colores
    private void OnCollisionEnter(Collision collision)
    {        

        //Si un cubo de color choca contra el engache de la grua, se queda unio a el media un joint
        if ((this.gameObject.tag == "cuboRojo" || this.gameObject.tag == "cuboAzul") && collision.gameObject.tag == "enganche" && GameObject.FindGameObjectWithTag("grua").GetComponent<moveCrane>().peso == null) {

            //Se crea un joint con unos ciertos valores
            this.gameObject.AddComponent<HingeJoint>();
            this.gameObject.GetComponent<HingeJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();
            this.gameObject.GetComponent<HingeJoint>().axis = new Vector3(1f,1f,1f);

            //Se actualiza el valor de "peso" en la grua
            GameObject.FindGameObjectWithTag("grua").GetComponent<moveCrane>().peso = this.gameObject;
            this.gameObject.GetComponent<HingeJoint>().autoConfigureConnectedAnchor = false;
            this.gameObject.GetComponent<HingeJoint>().connectedAnchor = new Vector3(0, -0.5f, 0);
        }

        //Si un cubo de color choca contra su pozo correspondiente 
        if (this.gameObject.tag == "pozoRojo" && collision.gameObject.tag == "cuboRojo") {
            Destroy(collision.gameObject);
        }
        if (this.gameObject.tag == "pozoAzul" && collision.gameObject.tag == "cuboAzul"){
            Destroy(collision.gameObject);
        }

    }
}
