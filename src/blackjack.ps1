[CmdletBinding()]
param (
    [Parameter(HelpMessage = "URL til kortstokk", Mandatory = $false)]
    [string]
    $urlKortstokk = 'http://nav-deckofcards.herokuapp.com/shuffle'
)

$ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop

$BlackJack = { 21 }

$function:cardScore = { param($card)
    switch ($card.value) {
        { $_ -cin @('J', 'Q', 'K') } { 10 }
        'A' { 11 }
        default { $_ }
    }
}

$function:cardShow = { param($card) $card.suit[0] + $card.value }

$function:docScore = { param ([object[]] $doc)

    $function:sloop = { param([object[]]$doc, $sum)
        if ($doc.Count -eq 1) { $sum + (cardScore $doc[0]) }
        else { sloop ($doc[1..$doc.Count]) ($sum + (cardScore $doc[0])) }
    }

    (sloop $doc 0)
}

$function:docShow = { param([object[]]$doc)

    $function:hloop = { param([object[]]$doc, $show)
        if ($doc.Count -eq 1) { $show + (cardShow $doc[0]) }
        else { hloop ($doc[1..$doc.Count]) ($show + (cardShow $doc[0]) + ',') }
    }
    
    (hloop $doc '')
}

$function:noCards = { param([object[]]$doc) $doc.Count -eq 0 }
$function:isLT17 = { param([object[]]$doc) (docScore $doc) -lt 17 }
$function:isBJ = { param([object[]]$doc) (docScore $doc) -eq (&$BlackJack) }
$function:isGTBJ = { param([object[]]$doc) (docScore $doc) -gt (&$BlackJack) }

$function:giveReceiveX = { param($x, [object[]]$g, [object[]]$r)
    $t = $g[0..($x - 1)]
    ($g[2..$g.Count]), ($r + $t)
}

$function:bjResult = { param($winner, [object[]]$me, [object[]]$magnus)
    [ordered]@{  
        winner = $winner
        me     = [ordered]@{ score = (docScore $me); cards = (docShow $me) } 
        magnus = [ordered]@{ score = (docScore $magnus); cards = (docShow $magnus) } 
    } | ConvertTo-Json
}

function blackjack {
    param(
        $me = @(),
        $magnus = @(),
        $doc = ((Invoke-WebRequest -Uri $urlKortstokk).Content | ConvertFrom-Json)
    )

    $function:bjloop = { param($me, $magnus, $doc)

        $function:giveMe = { param($x)
            $d, $e = (giveReceiveX $x $doc $me)
            bjloop $e $magnus $d
        }

        $function:giveMagnus = { param($x)
            $d, $a = (giveReceiveX $x $doc $magnus)
            bjloop $me $a $d
        }

        $MagnusLEMe = { (docScore $magnus) -le (docScore $me) }
        $winMe = { (bjResult 'me' $me $magnus) }
        $winMagnus = { (bjResult 'magnus' $me $magnus) }

        switch (0) {
            { (noCards $me) } { giveMe 2; break }
            { (noCards $magnus) } { giveMagnus 2; break }
            { (isBJ $me) } { return (&$winMe) }
            { (isBJ $magnus) } { return (&$winMagnus) }
            { (isLT17 $me) } { giveMe 1; break }
            { (isGTBJ $me) } { return (&$winMagnus) }
            { (&$MagnusLEMe) } { giveMagnus 1; break }
            { (isGTBJ $magnus) } { return (&$winMe) }
            Default { return (&$winMagnus) }
        }
    }

    bjloop $me $magnus $doc
}