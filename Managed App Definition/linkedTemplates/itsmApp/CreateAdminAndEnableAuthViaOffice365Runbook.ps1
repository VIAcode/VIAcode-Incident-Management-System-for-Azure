#Licence URLs:
#https://www.viacode.com/viacode-incident-management-license/
#https://www.viacode.com/gnu-affero-general-public-license/
workflow CreateAdminAndEnableAuthViaOffice365Runbook
{
    Param(
        [Parameter(Mandatory = $true)]
        [String] $siteDomain
    )

    $Url = "$siteDomain/"

    $sleepSeconds = 10
    $iterationCount = 90
    $requestTimeoutSeconds = 60

    $iteration = 0

    $doesSiteWork = $true

    try {
        $webrequest = Invoke-WebRequest -Uri $url -TimeoutSec $requestTimeoutSeconds -UseBasicParsing
    }
    catch {
        $doesSiteWork = $false
    }

    while (!$doesSiteWork -and ($iteration -lt $iterationCount)) {
        Start-Sleep -s $sleepSeconds
        $doesSiteWork  = $true
        try {
            $webrequest = Invoke-WebRequest -Uri $url -TimeoutSec $requestTimeoutSeconds -UseBasicParsing
        }
        catch {
            $doesSiteWork = $false
        }
        $iteration = $iteration + 1
    }

    if ($doesSiteWork) {
        $CredentialAssetName = 'VimsAzureAppCredential'

        #Get the credential with the above name from the Automation Asset store
        $Cred = Get-AutomationPSCredential -Name $CredentialAssetName

        $CredentialAssetName = 'VimsAdminCredential'

        #Get the credential with the above name from the Automation Asset store
        $vimsAdminCred = Get-AutomationPSCredential -Name $CredentialAssetName
        if(!$vimsAdminCred) {
            Throw "Could not find an Automation Credential Asset named '${CredentialAssetName}'. Make sure you have created one in this Automation Account."
        }
    
        InlineScript {
            $Url = "$Using:siteDomain/"
            $webrequest = Invoke-WebRequest -Uri $url -UseBasicParsing -SessionVariable websession
        
            $headers = @{
                'Content-Type' = 'application/json';
                'X-CSRF-Token' = $webrequest.Headers['CSRF-TOKEN']
            }
            $Url = "$Using:siteDomain/api/v1/users"
            $vimsAdminCred = $Using:vimsAdminCred
            $Body = "{
                ""email"": ""$($vimsAdminCred.UserName)"",
                ""firstname"": ""Admin"",
                ""id"": ""c-0"",
                ""lastname"": ""User"",
                ""password"": ""$($vimsAdminCred.GetNetworkCredential().Password)"",
                ""role_ids"": []        
            }"
            Invoke-RestMethod -Method 'Post' -Uri $url -Headers $headers -Body $body -WebSession $websession
        }

        if ($Cred) {
            $Bytes = [System.Text.Encoding]::UTF8.GetBytes("$($VimsAdminCred.UserName):$($VimsAdminCred.GetNetworkCredential().Password)")
            $EncodedCreds =[Convert]::ToBase64String($Bytes)

            $headers = @{
                'Authorization' = "Basic $EncodedCreds";
                'Content-Type' = 'application/json'
            }

            $Url = "$siteDomain/api/v1/settings/22"
            $Body = "{
                ""id"": 22,
                ""name"": ""http_type"",
                ""state_current"": {
                    ""value"": ""https""
                }
            }"
            Invoke-RestMethod -Method 'Put' -Uri $url -Headers $headers -Body $body
                        
            $Url = "$siteDomain/api/v1/settings/64"
            $Body = "{
                ""id"": 64,
                ""name"": ""auth_microsoft_office365"",
                ""state_current"": {
                    ""value"": true
                }
            }"
            Invoke-RestMethod -Method 'Put' -Uri $url -Headers $headers -Body $body
    
            $userName = $Cred.UserName
            $pwd = $Cred.GetNetworkCredential().Password
    
            $Url = "$siteDomain/api/v1/settings/65"
            $Body = "{
                ""id"": 65,
                ""name"": ""auth_microsoft_office365_credentials"",
                ""state_current"": {
                    ""value"": {
                        ""app_id"": ""$userName"",
                        ""app_secret"": ""$pwd""
                    }
                }
            }"

            Invoke-RestMethod -Method 'Put' -Uri $url -Headers $headers -Body $body
        }
    }
}