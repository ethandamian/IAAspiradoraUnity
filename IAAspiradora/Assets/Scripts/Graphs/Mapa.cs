using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour{

    public Transform grafica;
    public Grafica mapa = new Grafica();
    public Vertice preV = null, aux, encontrado = null;
    public Vertice anterior = null, stack;
    public GameObject nodo, newNodo;
    public int cont = -1, tam;
    public bool canSetNode = true;
    public float granularidad;

    public Vertice baseCarga;

    Vector3 vaux;

    public Stack<Vertice> dfsStack = new Stack<Vertice>();

    // Update is called once per frame
    void Update(){
        DrawGraph();
        if (mapa.camino.Count > 0) {
            DrawPath();
        }
    }

    /**
     * M�todo que coloca un nodo de la gr�fica en la posici�n que se necesite
     * 0 al centro del objeto
     * 1 a la izquierda
     * 2 a al frente
     * 3 a la derecha
     * La posici�n se determina con un offset de "granularidad"
     * Si ya existe un v�rtice en esa posici�n no lo coloca y lo hace adyacente al existente.
     */
    public void ColocarNodo(int direccion) {
        if (cont >= 0) {
            switch (direccion) {
                case 0://Centro
                foreach (Vertice v in mapa.grafica) {
                    if (Vector3.Distance(this.gameObject.transform.position, v.posicion) < granularidad) {
                        canSetNode = false;
                        break;
                    }
                }
                break;
                case 1://Izquierda
                vaux = transform.position + this.transform.right * -1 * granularidad;
                foreach (Vertice v in mapa.grafica) {
                    if (Vector3.Distance(vaux, v.posicion) < granularidad - 0.15) {//Verifica si alg�n v�rtice est� al menos as� de cerca.
                        encontrado = v;
                        canSetNode = false;
                        break;
                    }
                }
                break;
                case 2://Frente
                vaux = transform.position + this.transform.forward * granularidad;
                foreach (Vertice v in mapa.grafica) {
                    if (Vector3.Distance(vaux, v.posicion) < granularidad - 0.15) {
                        encontrado = v;
                        canSetNode = false;
                        break;
                    }
                }
                break;
                case 3://Derecha
                vaux = transform.position + this.transform.right  * granularidad;
                foreach (Vertice v in mapa.grafica) {
                    if (Vector3.Distance(vaux, v.posicion) < granularidad - 0.15) {
                        encontrado = v;
                        canSetNode = false;
                        break;
                    }
                }
                break;
            }
        }
        if (canSetNode) {
            cont++;
            switch (direccion) {
                case 0://Centro
                newNodo = Instantiate(nodo, this.gameObject.transform.position, Quaternion.identity);//Crea un objeto de visualizaci�n, no es objeto de la gr�fica.
                newNodo.transform.SetParent(grafica);
                aux = new Vertice(cont, this.transform.position);
                preV = aux;     
                baseCarga = aux;           
                break;
                case 1://Izquierda
                vaux = transform.position + this.transform.right * -1 * granularidad;
                newNodo = Instantiate(nodo, vaux, Quaternion.identity);
                newNodo.transform.SetParent(grafica);
                aux = new Vertice(cont, vaux);
                break;
                case 2://Frente
                vaux = transform.position + this.transform.forward * granularidad;
                newNodo = Instantiate(nodo, vaux, Quaternion.identity);
                newNodo.transform.SetParent(grafica);
                aux = new Vertice(cont, vaux);
                break;
                case 3://Derecha
                vaux = transform.position + this.transform.right * granularidad;
                newNodo = Instantiate(nodo, vaux, Quaternion.identity);
                newNodo.transform.SetParent(grafica);
                aux = new Vertice(cont, vaux);
                break;
            }
            dfsStack.Push(aux);

            if (cont > 0) {
                preV.AgregarVecino(aux);
                aux.AgregarVecino(preV);
                aux.setPadre(preV);
            }

            newNodo.GetComponent<Tag>().setId(cont);
            mapa.AgregarVertice(aux);
            encontrado = null;
        }
        if (encontrado != null) {
            preV.AgregarVecino(encontrado);
            encontrado.AgregarVecino(preV);
        }
        canSetNode = true;
        //Debug.Log(mapa.toString());
    }

    //Asigna el v�rtice anterior.
    public void setPreV(Vertice newPreV) {
        preV = newPreV;
    }

    //Verifica si el Stack est� vac�o.
    public bool isEmptyStack() {
        return dfsStack.Count == 0 ? true : false;
    }

    //Verifica si es posible hacer pop al stack, si es posible regresa el objeto en out Vertice
    public bool popStack(out Vertice stack) {
        stack = null;
        return dfsStack.TryPop(out stack);
    }

    //Dibuja la gr�fica en Scene
    public void DrawGraph() {
        if (cont >= 0) {
            foreach (Vertice g in mapa.grafica) {
                foreach (Vertice v in g.vecinos) {
                    Debug.DrawLine(g.posicion, v.posicion, Color.red);
                }
            }
        }
    }

    //Dibuja el path de A* en Scene
    public void DrawPath() {
        List<Vertice> aux = mapa.camino;
        for (int i = aux.Count - 1; i > 0; i--) {
            Debug.DrawLine(aux[i].posicion,aux[i].camino.posicion,Color.green);
        }
    }

    //Borra el camino en gr�fica
    public void clearPath() {
        mapa.camino.Clear();
    }

    //Dibuja los sensores en las posiciones que verifican los v�rtices.
    public void OnDrawGizmosSelected() {
        Vector3 iz = new Vector3(transform.position.x - granularidad, transform.position.y, transform.position.z);        
        Vector3 fr = new Vector3(transform.position.x, transform.position.y, transform.position.z + granularidad);
        Vector3 de = new Vector3(transform.position.x + granularidad, transform.position.y, transform.position.z);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + this.transform.right * -1*granularidad, 0.6f);
        Gizmos.DrawWireSphere(transform.position + this.transform.forward * granularidad, 0.6f);
        Gizmos.DrawWireSphere(transform.position + this.transform.right * granularidad, 0.6f);
    }

}
