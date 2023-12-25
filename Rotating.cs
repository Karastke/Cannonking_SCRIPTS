using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    public GameObject player; // Reference to the Player object
    public Camera mainCamera; // Reference to Main Camera
    public float orbitRadius = 2.0f; // Distance from Player object to "parrying_bar" object
    public Vector3 initialOffset; // Initial offset of the parrying_bar from the pivot

    private Vector3 orbitPosition; // Calculated orbit position

    void Start()
    {
        mainCamera = Camera.main; // Get reference to Main Camera
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition; // Get mouse position
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition); // Convert to world position
        mousePosition.z = player.transform.position.z; // Adjust Z position to match player's

        Vector2 direction = new Vector2(mousePosition.x - player.transform.position.x, mousePosition.y - player.transform.position.y); // Calculate direction vector
        direction.Normalize(); // Normalize the direction vector

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Determine rotation angle
        orbitPosition = Quaternion.Euler(0, 0, angle) * new Vector3(orbitRadius, 0, 0); // Calculate orbit position
        transform.position = player.transform.position + orbitPosition + initialOffset; // Update position
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Update rotation
    }
}
