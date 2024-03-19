using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Componente auxiliar para genera rayos que detecten colisiones de manera lineal
// En el script actual dibuja y comprueba colisiones con un rayo al frente del objeto
// y a un costado, sin embargo, es posible definir más rayos de la misma manera.
public class Rayo : MonoBehaviour{

    public float longitudDeRayo,radio;
    public LayerMask obstaculos;
    private bool frenteLibre, izquierdaLibre, derechaLibre;


    void Update(){
        // Se muestra el rayo únicamente en la pantalla de Escena (Scene)
        Debug.DrawLine(transform.position, transform.position + (transform.forward * longitudDeRayo), Color.blue);
        Debug.DrawLine(transform.position, transform.position + (transform.right * longitudDeRayo), Color.blue);
        Debug.DrawLine(transform.position, transform.position + (transform.right*-1 * longitudDeRayo), Color.blue);
    }

    void FixedUpdate(){
        // Similar a los métodos OnTrigger y OnCollision, se detectan colisiones con el rayo:
        frenteLibre = true;
        izquierdaLibre = true;
        derechaLibre = true;
        RaycastHit raycastFront;
        if (Physics.Raycast(transform.position, transform.forward, out raycastFront, longitudDeRayo, obstaculos)) {
            frenteLibre = false;
        }
        RaycastHit raycastLeft;
        if (Physics.Raycast(transform.position, transform.right * -1, out raycastLeft, longitudDeRayo, obstaculos)) {
            izquierdaLibre = false;
        }
        RaycastHit raycastRigth;
        if (Physics.Raycast(transform.position, transform.right, out raycastRigth, longitudDeRayo, obstaculos)) {
            derechaLibre = false;
        }
        Collider[] fColliders = Physics.OverlapSphere(transform.position + (transform.forward * longitudDeRayo), radio, obstaculos);
        if (fColliders.Length > 0) {
            frenteLibre = false;
        }
        Collider[] iColliders = Physics.OverlapSphere(transform.position + (transform.right * -1 * longitudDeRayo), radio, obstaculos);
        if (iColliders.Length > 0) {
            izquierdaLibre = false;
        }
        Collider[] dColliders = Physics.OverlapSphere(transform.position + (transform.right * longitudDeRayo), radio, obstaculos);
        if (dColliders.Length > 0) {
            derechaLibre = false;
        }
    }

    // Ejemplo de métodos públicos que podrán usar otros componentes (scripts):
    public bool FrenteLibre(){
        return frenteLibre;
    }
    public bool IzquierdaLibre() {
        return izquierdaLibre;
    }
    public bool DerechaLibre() {
        return derechaLibre;
    }
}
