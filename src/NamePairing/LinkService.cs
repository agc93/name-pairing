using Flurl;
using Microsoft.AspNetCore.Components;

namespace NamePairing;

public class LinkService
{
    private readonly NavigationManager _nav;

    public LinkService(NavigationManager navigationManager) {
        _nav = navigationManager;
    }

    public string GetLink(string recipientName, ShareLink link) {
        var fragment = link.GetLinkFragment();
        var url = new Url(_nav.BaseUri) {
            Path = $"/go/{recipientName}/" + fragment
        };
        return url.ToString();
    }
}