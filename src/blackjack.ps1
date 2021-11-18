[CmdletBinding()]
param (
    [Parameter(HelpMessage = "URL til kortstokk", Mandatory = $false)]
    [string]
    $urlKortstokk = 'http://nav-deckofcards.herokuapp.com/shuffle'
)

$ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop

$BlackJack = 21

$function:cardScore = { param($card)
    switch ($card.value) {
        { $_ -cin @('J', 'Q', 'K') } { 10 }
        'A' { 11 }
        default { $_ }
    }
}

$function:score = {
    param ([object[]] $doc)

    $function:sloop = { param([object[]]$doc, $sum)
        if ($doc.Count -eq 1) { $sum + (cardScore $doc[0]) }
        else { sloop ($doc[1..$doc.Count]) ($sum + (cardScore $doc[0])) }
    }

    (sloop $doc 0)
}

$function:noCards = { param([object[]]$doc) $doc.Count -eq 0 }
$function:isLT17 = { param([object[]]$doc) (score $doc) -lt 17 }
$function:isBJ = { param([object[]]$doc) (score $doc) -eq $BlackJack }
$function:isGTBJ = { param([object[]]$doc) (score $doc) -gt $BlackJack }

function recursive {
    param(
        $me = @(),
        $magnus = @(),
        $doc = ((Invoke-WebRequest -Uri $urlKortstokk).Content | ConvertFrom-Json)
    )

    function bjloop {
        param($me, $magnus, $doc)

        $function:giveReceiveX = { param($x, [object[]]$g, [object[]]$r)
            $t = $g[0..($x - 1)]
            ($g[2..$g.Count]), ($r + $t)
        }

        $function:meX = { param($x)
            $d, $e = (giveReceiveX $x $doc $me)
            bjloop $e $magnus $d
        }

        $function:magnusX = { param($x)
            $d, $a = (giveReceiveX $x $doc $magnus)
            bjloop $me $a $d
        }

        $function:isLEMe = { param([object[]]$doc) (score $doc) -le (score $me) }

        switch (0) {
            { (noCards $me) } { meX 2; break }
            { (noCards $magnus) } { magnusX 2; break }
            { (isBJ $me) } { return @{w = "me"; me = $me; magnus = $magnus } }
            { (isBJ $magnus) } { return @{w = "magnus"; me = $me; magnus = $magnus } }
            { (isLT17 $me) } { meX 1; break }
            { (isGTBJ $me) } { return @{w = "magnus"; me = $me; magnus = $magnus } }
            { (isLEMe $magnus) } { magnusX 1; break }
            { (isGTBJ $magnus) } { return @{w = "me"; me = $me; magnus = $magnus } }
            Default { return @{w = "magnus"; me = $me; magnus = $magnus } }
        }
    }

    bjloop $me $magnus $doc
}