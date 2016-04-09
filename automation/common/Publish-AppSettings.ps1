[CmdletBinding()]
param
(
    [Parameter(Mandatory=$True, Position=0, HelpMessage="Resource Group.")]
    $AzureLocation,
    [Parameter(Mandatory=$True, Position=1, HelpMessage="Resource Group.")]
    $ResourceGroupName,
    [Parameter(Mandatory=$True, Position=2, HelpMessage="Site Name")]
    $SiteName,
    [Parameter(Mandatory=$True, Position=3, HelpMessage="'appsetings' or 'conectionstrings'.")]
    $Settings,
    [Parameter(Mandatory=$True, Position=4, HelpMessage="Properties.")]
    $Properties
)

New-AzureRmResource -Location $AzureLocation -PropertyObject $Properties -ResourceGroupName $ResourceGroupName -ResourceType Microsoft.Web/sites/config -ResourceName "$sitename/$settings" -ApiVersion 2015-08-01 -Force
