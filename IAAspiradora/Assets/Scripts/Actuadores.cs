using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuadores : MonoBehaviour
{

    private Rigidbody rb; // Componente para simular acciones físicas realistas
    private Bateria bateria; // Componente adicional (script) que representa la batería
    private Sensores sensor; // Componente adicional (script) para obtener información de los sensores

    public float vel, velRot;
    private float upForce; // Indica la fuerza de elevación del dron
    private float movementForwardSpeed = 100.0f; // Escalar para indicar fuerza de movimiento frontal
    private float wantedYRotation; // Auxiliar para el cálculo de rotación
    private float currentYRotation; // Auxiliar para el cálculo de rotación
    private float rotateAmountByKeys = 2.5f; // Auxiliar para el cálculo de rotación
    private float rotationYVelocity; // Escalar (calculado) para indicar velocidad de rotación
    private float sideMovementAmount = 250.0f; // Escalar para indicar velocidad de movimiento lateral

    // Asignaciones de componentes
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sensor = GetComponent<Sensores>();
        bateria = GameObject.Find("Bateria").gameObject.GetComponent<Bateria>();
    }

    // ========================================
    // A partir de aqui, todos los métodos definidos son públicos, la intención
    // es que serán usados por otro componente (Controlador)

    /**
     * Mueve el gameObject hacia adelante a razón de vel
     */
    public void Adelante()
    {
        this.gameObject.transform.Translate(0, 0, vel * Time.deltaTime);
    }

    /**
     * Mueve el gameObject hacia atras a razón de vel
     */
    public void Atras()
    {
        this.gameObject.transform.Translate(0, 0, -vel * Time.deltaTime);
    }

    /**
     * Gira el gameObject a razón de 90 grados a la derecha
     */
    public void GirarDerecha()
    {
        transform.Rotate(Vector3.up, 90.0f);
    }

    /**
     * Gira el gameObject a razón de 90 grados a la izquierda
     */
    public void GirarIzquierda()
    {
        transform.Rotate(Vector3.up, -90.0f);
    }

    /**
     * Mueve el gameObject una cantidad a la derecha. (usando fuerzas)
     */
    public void Derecha()
    {
        rb.AddRelativeForce(Vector3.right * sideMovementAmount);
    }

    /**
     * Mueve el gameObject una cantidad a la izquierda. (usando fuerzas)
     */
    public void Izquierda()
    {
        rb.AddRelativeForce(Vector3.left * sideMovementAmount);
    }

    /**
     * Hace la velocidad del gameObject igual a 0.
     */
    public void Detener()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    /**
     * Desactiva de la escena el GameObject pasado como parámetro, y restablece las banderas.
     */
    public void Limpiar(GameObject basura)
    {
        basura.SetActive(false);
        sensor.SetTocandoBasura(false);
        sensor.SetCercaDeBasura(false);
    }

    /**
     * Llama a laa función CargarBateria del script de Bateria.
     */
    public void CargarBateria()
    {
        bateria.Cargar();
    }
}
