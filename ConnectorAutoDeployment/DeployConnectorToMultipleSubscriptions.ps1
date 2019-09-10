Clear-AzContext -Force

if (Login-AzAccount) {       
    if ($subscription = Get-AzSubscription | where State -eq Enabled | select Name, Id | Out-GridView -Title "Select subscription that VIAcode Incident Management System for Azure is deployed to" -OutputMode Single) {
        Select-AzSubscription -Subscription $subscription.Id | Out-Null
        $managedApps = Get-AzManagedApplication | where { $_.Properties.outputs.connectorName.Value.Length -gt 0 }
        $managedApp = $null
        if (-not ($managedApps -is [array])) {
            $managedApp = $managedApps
        } else {
            $managedAppInfo = $managedApps | 
                Add-Member -MemberType MemberSet `
                           -Name PSStandardMembers `
                           -Value ([System.Management.Automation.PSPropertySet]::new(
                                    'DefaultDisplayPropertySet',
                                    [string[]]('Name','ResourceGroupName')
                                )) `
                           -PassThru | 
                Out-GridView -Title "Select VIAcode Incident Management System for Azure managed application" -OutputMode Single
            if ($managedAppinfo) {
                $managedApp = Get-AzManagedApplication -ResourceGroupName $managedAppInfo.ResourceGroupName
            }
        }

        if ($managedApp) {
            $jobs = @()   

            Write-Progress -Activity "Getting list of subscriptions that haven't been connected to VIAcode Incident Management System for Azure yet" -PercentComplete -1
            $agRequestJobs = Get-AzSubscription | where State -eq Enabled | where { -not ($_.Id -eq $subscription.Id) } | ForEach-Object {  
                Select-AzSubscription -Subscription $_.Id | Out-Null
                Start-Job -InitializationScript {Import-Module Az.Accounts} -ScriptBlock {
                    param($subscription, $managedApp, $currentAzureContext)
                    $azureRmProfile = [Microsoft.Azure.Commands.Common.Authentication.Abstractions.AzureRmProfileProvider]::Instance.Profile
                    $profileClient = New-Object Microsoft.Azure.Commands.ResourceManager.Common.RMProfileClient($azureRmProfile)
   
                    $token = $profileClient.AcquireAccessToken($currentAzureContext.Tenant.TenantId)

                    $ags = Invoke-RestMethod -Method 'Get' -Uri "https://management.azure.com/subscriptions/$($subscription.Id)/providers/microsoft.insights/actionGroups?api-version=2018-03-01" -Headers @{"Authorization"="Bearer $($token.AccessToken)"}
                    if (($ags.value | where {$_.properties.azureFunctionReceivers.functionAppResourceId -eq "$($managedApp.Properties.managedResourceGroupId)/providers/Microsoft.Web/sites/$($managedApp.Properties.outputs.connectorName.Value)"} | measure).Count -eq 0) {
                        $subscription
                    }
                } -ArgumentList $_, $managedApp, (Get-AzContext)
            }            
            Wait-Job -Job $agRequestJobs | Out-Null   
            Write-Progress -Activity "Getting list of subscriptions that haven't been connected to VIAcode Incident Management System for Azure yet" -Completed    
            $notConnectedSubscriptions = Receive-Job -Job $agRequestJobs
                       
            $input = Read-Host "`nDo you want to connect to VIAcode Incident Management System for Azure (A)ll your subscriptions that haven't been connected yet or (S)elect subscriptions that you want to connect? (Default is All)"
            if ($input -eq 's') {
                $selectedSubscriptions = $notConnectedSubscriptions | select Name, Id | Out-GridView -Title "Select subscriptions that you want to connect to VIAcode Incident Management System for Azure" -PassThru 
            } else {
                $selectedSubscriptions = $notConnectedSubscriptions
            }
                
            if ($selectedSubscriptions) {
                $input = Read-Host "`nDo you want to use (D)efault location or (S)elect one?"
                if ($input -eq 's') {
                    $location = (Get-AzLocation | select @{Name = "Location"; Expression = {$_.DisplayName}}, @{Name = "Id"; Expression = {$_.Location}} | Add-Member -MemberType MemberSet -Name PSStandardMembers -Value ([System.Management.Automation.PSPropertySet]::new('DefaultDisplayPropertySet', [string[]]('Location'))) -PassThru | 
                        Out-GridView -Title "Select location" -OutputMode Single).Id
                } else {
                    $location = $managedApp.Location
                }
                if ($location) {
                    Select-AzSubscription -Subscription $selectedSubscriptions[0].Id | Out-Null                    

                    $terms = Get-AzMarketplaceTerms -Publisher "viacode_consulting-1089577" -Product "viacode-itsm-z-preview" -Name "itsm-z-paid"
                        
                    $input = Read-Host "`nBy deploying alert connector via this script you automatically accept the following:`n`nTerms of use: https://catalogartifact.azureedge.net/publicartifacts/viacode_consulting-1089577.viacode-itsm-z-preview-aefc5caa-174f-49fe-97c5-22e9c894ab2d/termsOfUse.html`nPrivacy policy: $($terms.PrivacyPolicyLink)`nAzure Marketplace Terms: https://azure.microsoft.com/support/legal/marketplace-terms/`n`nDo you want to proceed? (Y/N)"

                    if ($input -eq 'y') {
                        Write-Progress -Activity "Deploying VIAcode Incident Management System for Azure connectors" -PercentComplete -1
                    
                        $selectedSubscriptions | ForEach-Object {
                            Select-AzSubscription -Subscription $_.Id | Out-Null
                                                                            
                            Get-AzMarketplaceTerms -Publisher "viacode_consulting-1089577" -Product "viacode-itsm-z-preview" -Name "itsm-z-paid" | Set-AzMarketplaceTerms -Accept | Out-Null

                            $date = Get-Date -Format "yyyyMMddHHmmss"
                            try {
                                $resourceGroup = "Connector-" + [Guid]::NewGuid()
                                if ((Get-AzResourceGroup | where {$_.ResourceGroupName -eq $resourceGroup}).Count -eq 0) {
                                    New-AzResourceGroup -Name $resourceGroup -Location $location -ErrorAction Stop | Out-Null
                                }
                                # Create the managed application
                                $jobs += Start-Job -ScriptBlock {
                                    param($dir, $date, $resourceGroup, $functionSubscription, $functionResourceGroup, $functionName, $location, $context)                                 

                                    $tries = 0
                                    while ($true) {
                                        $tries++
                                        try {                                        
                                            New-AzResourceGroupDeployment -Name "viacode_consulting-1089577.viacode-itsm-z-preview-$date" -ResourceGroupName $resourceGroup -TemplateUri "https://raw.githubusercontent.com/VIAcode/VIAcode-Incident-Management-System-for-Azure/develop/ConnectorAutoDeployment/template.json" -functionSubscription $functionSubscription -functionResourceGroup $functionResourceGroup -functionName $functionName -location $location -AzContext $context
                                            break
                                        } catch {
                                            if($tries -ge 30){
                                                Throw
                                                Exit
                                            }
                                            sleep -s (Get-Random -minimum 1 -maximum 6)
                                        }
                                    }
                                } -ArgumentList $dir, $date, $resourceGroup, $managedApp.Properties.managedResourceGroupId.Substring('/subscriptions/'.Length, '00000000-0000-0000-0000-000000000000'.Length), $managedApp.Properties.managedResourceGroupId.Substring('/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/'.Length), $managedApp.Properties.outputs.connectorName.Value, $location, (Get-AzContext)
                            } 
                            catch [Microsoft.Rest.Azure.CloudException] { }
                        }

                        if ($jobs.Count -gt 0) {
                            Wait-Job -Job $jobs | Out-Null   
                            Write-Progress -Activity "Deploying VIAcode Incident Management System for Azure connectors" -Completed
                            Receive-Job -Job $jobs
                        }
                    }
                }
            }
        }
    }

    Clear-AzContext -Force
}