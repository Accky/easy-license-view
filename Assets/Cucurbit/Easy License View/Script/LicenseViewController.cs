using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cucurbit
{
    public class LicenseViewController : MonoBehaviour
    {
        [SerializeField]
        EntityLicense licenses;

        [SerializeField]
        RectTransform rootTransform;
        [SerializeField]
        Text baseTitleText;
        [SerializeField]
        Provision baseProvisionText;

        private void Awake()
        {
            if (rootTransform != null) {
                if (licenses != null) {

                    licenses.param.ForEach(entity => {
                        //add license title and provision.
                        Text title = Instantiate(baseTitleText);
                        Provision provision = Instantiate(baseProvisionText);
                        title.text = entity.Title;
                        provision.ProvisionText.text = entity.Provision;
                        title.transform.SetParent(rootTransform, false);
                        title.transform.SetAsLastSibling();
                        //put last.
                        provision.transform.SetParent(rootTransform, false);
                        provision.transform.SetAsLastSibling();
                        //display
                        title.gameObject.SetActive(true);
                        provision.gameObject.SetActive(true);
                    });
                }
                else {
                    Debug.Log("license.asset is no set.");
                }
            }
        }
    }
}
