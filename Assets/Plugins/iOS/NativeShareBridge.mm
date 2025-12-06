
#import <Foundation/Foundation.h>
#include "UnityFramework/UnityFramework-Swift.h"

extern "C" {

#pragma mark - Functions


void _shareText(char* text){
    NSString *a = [[NSString alloc] initWithUTF8String:text];
    UIViewController *vc = UnityGetGLViewController();
    [[NativeShare shared] ShareTextWithText:a vc:vc];
}

void _shareFile(char* paths, char* message){
    NSString *convertedUrl = [[NSString alloc] initWithUTF8String:paths];
    NSString *convertedMessage = [[NSString alloc] initWithUTF8String:message];
    UIViewController *vc = UnityGetGLViewController();
    [[NativeShare shared] ShareFileWithPaths:convertedUrl message:convertedMessage vc:vc];
}

/*char* cStringCopy(const char* string){
 if (string == NULL){
 return NULL;
 }
 char* res = (char*)malloc(strlen(string)+1);
 strcpy(res, string);
 return res;
 }*/

}


