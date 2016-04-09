Function Check-PowerShell()
{
    # Check if we're running in the PowerShell ISE or PowerShell Console.
    If ($Host.Name -like "*ISE*")
    {
        $ISE = $True
        # Console output
        Write-Verbose -Message "[Information] Running in PowerShell ISE."
    }
    Else # Executing from the PowerShell Console instead of the PowerShell ISE.
    {
        $ISE = $False
        # Console output
        Write-Verbose -Message "[Information] Running in PowerShell Console."
    }
    Return $ISE
} 

Export-ModuleMember -Function Check-PowerShell