using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

namespace DeathNote
{
    public class UIControllerScript : MonoBehaviour
    {

        public Label lblResult;
        public TextField txtPlayerUsername;
        public Button btnSubmit;
        public DropdownField dpdnDeathType;
        public TextField txtTimeOfDeath;
        public ProgressBar pbRemainingTime;
        //Label lblInstructions;
        public Label lblInstructions1;
        public Label lblInstructions2;
        public Label lblInstructions3;
        //Label lblSETitle;
        public Label lblSEDescription;
        public Label lblSEWarning;
        public Button btnActivateEyes;

        public void Init()
        {
            btnSubmit.clicked += BtnSubmitOnClick;
            txtPlayerUsername.RegisterCallback<KeyUpEvent>(txtPlayerUsernameOnValueChanged);
            btnActivateEyes.clicked += BtnActivateEyesOnClick;
        }

        private void txtPlayerUsernameOnValueChanged(KeyUpEvent evt)
        {
            if (evt.keyCode == KeyCode.Return)
            {
                BtnActivateEyesOnClick();
            }
        }

        private void BtnActivateEyesOnClick()
        {
            throw new NotImplementedException();
        }

        private void BtnSubmitOnClick()
        {
            throw new NotImplementedException();
        }
    }
}
