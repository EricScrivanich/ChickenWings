
import Foundation

@objc public class NativeShare: NSObject {
    @objc public static let shared = NativeShare()

    @objc public func ShareText(text: String, vc: UIViewController) {
        //print("Shard text is : " + text)

        let textToShare = [text]

        let activityViewController = UIActivityViewController(activityItems: textToShare, applicationActivities: nil)
        if let popoverController = activityViewController.popoverPresentationController {
            popoverController.sourceRect = CGRect(x: UIScreen.main.bounds.width / 2, y: UIScreen.main.bounds.height / 2, width: 0, height: 0)
            popoverController.sourceView = vc.view
            popoverController.permittedArrowDirections = UIPopoverArrowDirection(rawValue: 0)
        }

        DispatchQueue.main.async {
            vc.present(activityViewController, animated: true, completion: nil)
        }
    }

    @objc public func ShareFile(paths: String, message: String, vc: UIViewController) {
        var urls = [Any]()
        let pathsList = paths.components(separatedBy: "<smile123>")
        for i in 0 ..< pathsList.count {
            let url = NSURL.fileURL(withPath: pathsList[i])
            urls.append(url)
        }

        if message != "" {
            urls.append(message)
        }

        let activityViewController = UIActivityViewController(activityItems: urls, applicationActivities: nil)
        if let popoverController = activityViewController.popoverPresentationController {
            popoverController.sourceRect = CGRect(x: UIScreen.main.bounds.width / 2, y: UIScreen.main.bounds.height / 2, width: 0, height: 0)
            popoverController.sourceView = vc.view
            popoverController.permittedArrowDirections = UIPopoverArrowDirection(rawValue: 0)
        }

        DispatchQueue.main.async {
            vc.present(activityViewController, animated: true, completion: nil)
        }
    }
}
