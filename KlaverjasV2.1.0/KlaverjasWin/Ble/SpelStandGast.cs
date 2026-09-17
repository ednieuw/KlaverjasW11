using System.Text.Json.Serialization;

namespace Klaverjas.Ble;

/// <summary>Eén kaart zoals de gastheer hem stuurt (JSON-vorm van <c>KaartView</c>).</summary>
public sealed class KaartViewGast
{
    [JsonPropertyName("index")] public int Index { get; set; }

    [JsonPropertyName("naam")]
    [JsonConverter(typeof(TekenJsonConverter))]
    public char Naam { get; set; }

    [JsonPropertyName("kleur")] public int Kleur { get; set; }
    [JsonPropertyName("open")] public bool Open { get; set; }
    [JsonPropertyName("klikbaar")] public bool Klikbaar { get; set; }
    [JsonPropertyName("plek")] public int Plek { get; set; }
}

/// <summary>Eén gespeelde kaart binnen een slag (JSON-vorm van <c>SlagView</c>).</summary>
public sealed class SlagViewGast
{
    [JsonPropertyName("kleur")] public int Kleur { get; set; }

    [JsonPropertyName("naam")]
    [JsonConverter(typeof(TekenJsonConverter))]
    public char Naam { get; set; }

    /// <summary>Pos-code 1..5, zie <see cref="Klaverjas.Engine.Pos"/>.</summary>
    [JsonPropertyName("speler")] public int Speler { get; set; }

    /// <summary>Alleen interessant voor de statistiek op de gastheer; hier genegeerd.</summary>
    [JsonPropertyName("tactiek")] public int Tactiek { get; set; }
}

/// <summary>
/// De velden van een <c>STAND</c>-bericht die de gast-kant nodig heeft. Dit is
/// bewust niet de volledige <c>SpelView</c> van de gastheer (die heeft ook
/// statistiekvelden waar de gast niets aan heeft) — <c>System.Text.Json</c>
/// negeert onbekende sleutels vanzelf, dus dat hoeft hier niet nagebouwd.
///
/// Velden en betekenis: zie BLUETOOTH-VOOR-WINDOWS.md, "Wat er precies in een
/// STAND zit". Wat de gast zelf moet uitrekenen (<c>benIkAanZet</c>,
/// <c>mijnHand</c> e.d.) staat niet hier maar in <c>GastWeergave</c>.
/// </summary>
public sealed class SpelStandGast
{
    [JsonPropertyName("handZuid")] public List<KaartViewGast> HandZuid { get; set; } = new();
    [JsonPropertyName("handNoord")] public List<KaartViewGast> HandNoord { get; set; } = new();
    [JsonPropertyName("tafelZuid")] public List<KaartViewGast> TafelZuid { get; set; } = new();
    [JsonPropertyName("tafelNoord")] public List<KaartViewGast> TafelNoord { get; set; } = new();
    [JsonPropertyName("dichtZuid")] public List<KaartViewGast> DichtZuid { get; set; } = new();
    [JsonPropertyName("dichtNoord")] public List<KaartViewGast> DichtNoord { get; set; } = new();

    [JsonPropertyName("onderZuid")] public bool[] OnderZuid { get; set; } = new bool[4];
    [JsonPropertyName("onderNoord")] public bool[] OnderNoord { get; set; } = new bool[4];

    [JsonPropertyName("slag")] public List<SlagViewGast> Slag { get; set; } = new();
    [JsonPropertyName("vorigeSlag")] public List<SlagViewGast> VorigeSlag { get; set; } = new();

    /// <summary>0..3, of 999 = nog onbekend.</summary>
    [JsonPropertyName("troef")] public int Troef { get; set; } = 999;

    /// <summary>0 = nog onbekend, 1 = Zuid, 2 = Noord.</summary>
    [JsonPropertyName("troefmaker")] public int Troefmaker { get; set; }

    [JsonPropertyName("slagNr")] public int SlagNr { get; set; }

    /// <summary>Pos-code 1..4: wie moet er spelen.</summary>
    [JsonPropertyName("aanZet")] public int AanZet { get; set; }

    [JsonPropertyName("wachtOpSpeler")] public bool WachtOpSpeler { get; set; }
    [JsonPropertyName("troefVraag")] public bool TroefVraag { get; set; }

    /// <summary>
    /// Een slag is net afgelopen; wie dan ook mag "verder" tikken — niet
    /// gebonden aan <c>benIkAanZet</c>, zie <see cref="GastZet.VoorVerder"/>.
    /// </summary>
    [JsonPropertyName("wachtOpVerder")] public bool WachtOpVerder { get; set; }

    [JsonPropertyName("puntenZuid")] public int PuntenZuid { get; set; }
    [JsonPropertyName("puntenNoord")] public int PuntenNoord { get; set; }
    [JsonPropertyName("roemZuid")] public int RoemZuid { get; set; }
    [JsonPropertyName("roemNoord")] public int RoemNoord { get; set; }
    [JsonPropertyName("totaalZuid")] public long TotaalZuid { get; set; }
    [JsonPropertyName("totaalNoord")] public long TotaalNoord { get; set; }
    [JsonPropertyName("partijenZuid")] public int PartijenZuid { get; set; }
    [JsonPropertyName("partijenNoord")] public int PartijenNoord { get; set; }

    /// <summary>Voor de gast altijd 2 (Noord) — uit de data gelezen, niet aangenomen.</summary>
    [JsonPropertyName("mijnKant")] public int MijnKant { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; } = "";
    [JsonPropertyName("melding")] public string Melding { get; set; } = "";

    /// <summary>true zodra het spel is afgerekend: Punten/Roem hierboven zijn dan de eindstand.</summary>
    [JsonPropertyName("spelUit")] public bool SpelUit { get; set; }
}

/// <summary>
/// De keuze die de gast terugstuurt: precies drie vormen, nooit meer dan één
/// tegelijk (zie <see cref="VoorTroef"/>/<see cref="VoorKaart"/>/
/// <see cref="VoorVerder"/>). Velden die niet gezet zijn blijven weg uit de
/// JSON in plaats van als <c>null</c> mee te gaan — dat mag allebei van de
/// Mac-kant, maar dit is de kortere vorm.
/// </summary>
public sealed class GastZet
{
    [JsonPropertyName("troef")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Troef { get; set; }

    [JsonPropertyName("kaart")]
    [JsonConverter(typeof(TekenJsonConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public char? Kaart { get; set; }

    [JsonPropertyName("kleur")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Kleur { get; set; }

    /// <summary>
    /// "Volgende slag"-tik. Niet gebonden aan <c>benIkAanZet</c>: stuur dit
    /// alleen als de laatst ontvangen STAND <c>wachtOpVerder == true</c> had,
    /// van welke kant dan ook, wie het eerst is.
    /// </summary>
    [JsonPropertyName("verder")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Verder { get; set; }

    public static GastZet VoorTroef(int kleur) => new() { Troef = kleur };

    public static GastZet VoorKaart(char naam, int kleur) => new() { Kaart = naam, Kleur = kleur };

    public static GastZet VoorVerder() => new() { Verder = true };
}
