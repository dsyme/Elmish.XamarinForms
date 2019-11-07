namespace Fabulous.XamarinForms

open System
open FFImageLoading.Forms
open Fabulous
open FSharp.Data.Adaptive

module ViewAttributes =
    let CachedImageSourceAttribKey = AttributeKey "CachedImage_Source"
    let CachedImageLoadingPlaceholderAttribKey = AttributeKey "CachedImage_LoadingPlaceholder"
    let CachedImageErrorPlaceholderAttribKey = AttributeKey "CachedImage_ErrorPlaceholder"
    let CachedImageCacheTypeAttribKey = AttributeKey "CachedImage_CacheType"
    let CachedImageCacheDurationAttribKey = AttributeKey "CachedImage_CacheDuration"
    let CachedImageCacheKeyFactoryAttribKey = AttributeKey "CachedImage_CacheKeyFactory"
    let CachedImageLoadingDelayAttribKey = AttributeKey "CachedImage_LoadingDelay"
    let CachedImageLoadingPriorityAttribKey = AttributeKey "CachedImage_LoadingPriority"
    let CachedImageCustomDataResolverAttribKey = AttributeKey "CachedImage_CustomDataResolver"
    let CachedImageRetryCountAttribKey = AttributeKey "CachedImage_RetryCount"
    let CachedImageRetryDelayAttribKey = AttributeKey "CachedImage_RetryDelay"
    let CachedImageDownsampleWidthAttribKey = AttributeKey "CachedImage_DownsampleWidth"
    let CachedImageDownsampleHeightAttribKey = AttributeKey "CachedImage_DownsampleHeight"
    let CachedImageDownsampleToViewSizeAttribKey = AttributeKey "CachedImage_DownsampleToViewSize"
    let CachedImageDownsampleUseDipUnitsAttribKey = AttributeKey "CachedImage_DownsampleUseDipUnits"
    let CachedImageFadeAnimationEnabledAttribKey = AttributeKey "CachedImage_FadeAnimationEnabled"
    let CachedImageFadeAnimationDurationAttribKey = AttributeKey "CachedImage_FadeAnimationDuration"
    let CachedImageFadeAnimationForCachedImagesAttribKey = AttributeKey "CachedImage_FadeAnimationForCachedImages"
    let CachedImageBitmapOptimizationsAttribKey = AttributeKey "CachedImage_BitmapOptimizations"
    let CachedImageInvalidateLayoutAfterLoadedAttribKey = AttributeKey "CachedImage_InvalidateLayoutAfterLoaded"
    let CachedImageTransformPlaceholdersAttribKey = AttributeKey "CachedImage_TransformPlaceholders"
    let CachedImageTransformationsAttribKey = AttributeKey "CachedImage_Transformations"
    let CachedImageDownloadStartedAttribKey = AttributeKey "CachedImage_DownloadStarted"
    let CachedImageDownloadProgressAttribKey = AttributeKey "CachedImage_DownloadProgress"
    let CachedImageFileWriteFinishedAttribKey = AttributeKey "CachedImage_FileWriteFinished"
    let CachedImageFinishAttribKey = AttributeKey "CachedImage_Finish"
    let CachedImageSuccessAttribKey = AttributeKey "CachedImage_Success"
    let CachedImageErrorAttribKey = AttributeKey "CachedImage_Error"

open ViewAttributes

[<AutoOpen>]
module FFImageLoadingExtension =
    // Fully-qualified name to avoid extending by mistake
    // another View class (like Xamarin.Forms.View)
    type Fabulous.XamarinForms.View with
        // https://github.com/luberda-molinet/FFImageLoading/wiki/Xamarin.Forms-API
        /// Describes a CachedImage in the view
        // The inline keyword is important for performance
        static member inline CachedImage
            (?source: aval<Image>, ?aspect: aval<Xamarin.Forms.Aspect>, ?isOpaque: aval<bool>, // Align first 3 parameters with Image
             ?loadingPlaceholder: aval<Image>, ?errorPlaceholder: aval<Image>,
             ?cacheType, ?cacheDuration, ?cacheKeyFactory: aval<ICacheKeyFactory>,
             ?loadingDelay: aval<_>, ?loadingPriority: aval<_>,
             ?customDataResolver: aval<FFImageLoading.Work.IDataResolver>,
             ?retryCount: aval<_>, ?retryDelay: aval<_>,
             ?downsampleWidth: aval<_>, ?downsampleHeight: aval<_>, ?downsampleToViewSize: aval<_>, ?downsampleUseDipUnits: aval<_>,
             ?fadeAnimationEnabled: aval<_>, ?fadeAnimationDuration: aval<_>, ?fadeAnimationForCachedImages: aval<_>,
             ?bitmapOptimizations: aval<_>, ?invalidateLayoutAfterLoaded: aval<_>,
             ?transformPlaceholders: aval<_>, ?transformations: alist<_>,
             ?downloadStarted: aval<_>, ?downloadProgress: aval<_>, ?fileWriteFinished: aval<_>, ?finish: aval<_>, ?success: aval<_>, ?error: aval<_>,
             // inherited attributes common to all views
             ?gestureRecognizers, ?horizontalOptions, ?margin, ?verticalOptions, ?anchorX, ?anchorY, ?backgroundColor,
             ?behaviors, ?flowDirection, ?height, ?inputTransparent, ?isEnabled, ?isTabStop, ?isVisible, ?minimumHeight,
             ?minimumWidth, ?opacity, ?resources, ?rotation, ?rotationX, ?rotationY, ?scale, ?scaleX, ?scaleY, ?styles,
             ?styleSheets, ?tabIndex, ?translationX, ?translationY, ?visual, ?width, ?style, ?styleClasses, ?shellBackButtonBehavior,
             ?shellBackgroundColor, ?shellDisabledColor, ?shellForegroundColor, ?shellFlyoutBehavior, ?shellNavBarIsVisible,
             ?shellSearchHandler, ?shellTabBarBackgroundColor, ?shellTabBarDisabledColor, ?shellTabBarForegroundColor,
             ?shellTabBarIsVisible, ?shellTabBarTitleColor, ?shellTabBarUnselectedColor, ?shellTitleColor, ?shellTitleView,
             ?shellUnselectedColor, ?automationId, ?classId, ?effects, ?menu, ?ref, ?styleId, ?tag, ?focused, ?unfocused, ?created) =
            
            // Count the number of additional attributes
            let attribCount = 0
            let attribCount = match source with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match aspect with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match isOpaque with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match loadingPlaceholder with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match errorPlaceholder with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cacheType with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cacheDuration with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cacheKeyFactory with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match loadingDelay with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match loadingPriority with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match customDataResolver with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match retryCount with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match retryDelay with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleWidth with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleHeight with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleToViewSize with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleUseDipUnits with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fadeAnimationEnabled with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fadeAnimationDuration with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fadeAnimationForCachedImages with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match bitmapOptimizations with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match invalidateLayoutAfterLoaded with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match transformPlaceholders with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match transformations with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downloadStarted with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downloadProgress with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fileWriteFinished with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match finish with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match success with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match error with Some _ -> attribCount + 1 | None -> attribCount
    
            // Populate the attributes of the base element
            let attribs =
                ViewBuilders.BuildView(attribCount, ?gestureRecognizers=gestureRecognizers, ?horizontalOptions=horizontalOptions, ?margin=margin,
                    ?verticalOptions=verticalOptions, ?anchorX=anchorX, ?anchorY=anchorY, ?backgroundColor=backgroundColor, ?behaviors=behaviors,
                    ?flowDirection=flowDirection, ?height=height, ?inputTransparent=inputTransparent, ?isEnabled=isEnabled, ?isTabStop=isTabStop,
                    ?isVisible=isVisible, ?minimumHeight=minimumHeight, ?minimumWidth=minimumWidth, ?opacity=opacity, ?resources=resources,
                    ?rotation=rotation, ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?scaleX=scaleX, ?scaleY=scaleY, ?styles=styles,
                    ?styleSheets=styleSheets, ?tabIndex=tabIndex, ?translationX=translationX, ?translationY=translationY, ?visual=visual, ?width=width,
                    ?style=style, ?styleClasses=styleClasses, ?shellBackButtonBehavior=shellBackButtonBehavior, ?shellBackgroundColor=shellBackgroundColor,
                    ?shellDisabledColor=shellDisabledColor, ?shellForegroundColor=shellForegroundColor, ?shellFlyoutBehavior=shellFlyoutBehavior,
                    ?shellNavBarIsVisible=shellNavBarIsVisible, ?shellSearchHandler=shellSearchHandler, ?shellTabBarBackgroundColor=shellTabBarBackgroundColor,
                    ?shellTabBarDisabledColor=shellTabBarDisabledColor, ?shellTabBarForegroundColor=shellTabBarForegroundColor,
                    ?shellTabBarIsVisible=shellTabBarIsVisible, ?shellTabBarTitleColor=shellTabBarTitleColor, ?shellTabBarUnselectedColor=shellTabBarUnselectedColor,
                    ?shellTitleColor=shellTitleColor, ?shellTitleView=shellTitleView, ?shellUnselectedColor=shellUnselectedColor, ?automationId=automationId,
                    ?classId=classId, ?effects=effects, ?menu=menu, ?ref=ref, ?styleId=styleId, ?tag=tag, ?focused=focused, ?unfocused=unfocused, ?created=created)
                    
            let viewUpdater = ViewBuilders.UpdaterView (?gestureRecognizers=gestureRecognizers, ?horizontalOptions=horizontalOptions, ?margin=margin,
                                       ?verticalOptions=verticalOptions, ?anchorX=anchorX, ?anchorY=anchorY, ?backgroundColor=backgroundColor, ?behaviors=behaviors,
                                       ?flowDirection=flowDirection, ?height=height, ?inputTransparent=inputTransparent, ?isEnabled=isEnabled, ?isTabStop=isTabStop,
                                       ?isVisible=isVisible, ?minimumHeight=minimumHeight, ?minimumWidth=minimumWidth, ?opacity=opacity, ?resources=resources,
                                       ?rotation=rotation, ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?scaleX=scaleX, ?scaleY=scaleY, ?styles=styles,
                                       ?styleSheets=styleSheets, ?tabIndex=tabIndex, ?translationX=translationX, ?translationY=translationY, ?visual=visual, ?width=width,
                                       ?style=style, ?styleClasses=styleClasses, ?shellBackButtonBehavior=shellBackButtonBehavior, ?shellBackgroundColor=shellBackgroundColor,
                                       ?shellDisabledColor=shellDisabledColor, ?shellForegroundColor=shellForegroundColor, ?shellFlyoutBehavior=shellFlyoutBehavior,
                                       ?shellNavBarIsVisible=shellNavBarIsVisible, ?shellSearchHandler=shellSearchHandler, ?shellTabBarBackgroundColor=shellTabBarBackgroundColor,
                                       ?shellTabBarDisabledColor=shellTabBarDisabledColor, ?shellTabBarForegroundColor=shellTabBarForegroundColor,
                                       ?shellTabBarIsVisible=shellTabBarIsVisible, ?shellTabBarTitleColor=shellTabBarTitleColor, ?shellTabBarUnselectedColor=shellTabBarUnselectedColor,
                                       ?shellTitleColor=shellTitleColor, ?shellTitleView=shellTitleView, ?shellUnselectedColor=shellUnselectedColor, ?automationId=automationId,
                                       ?classId=classId, ?effects=effects, ?menu=menu, ?ref=ref, ?styleId=styleId, ?tag=tag, ?focused=focused, ?unfocused=unfocused, ?created=created)

            let downloadProgress = downloadProgress |> Option.map (AVal.map (fun f -> System.EventHandler<_>(fun _ args -> f args)))
            let downloadStarted = downloadStarted |> Option.map (AVal.map (fun f -> System.EventHandler<_>(fun _ args -> f args)))
            let fileWriteFinished = fileWriteFinished |> Option.map (AVal.map (fun f -> System.EventHandler<_>(fun _ args -> f args)))
            let finish = finish |> Option.map (AVal.map (fun f -> System.EventHandler<_>(fun _ args -> f args)))
            let success = success |> Option.map (AVal.map (fun f -> System.EventHandler<_>(fun _ args -> f args)))
            let error = error |> Option.map (AVal.map (fun f -> System.EventHandler<_>(fun _ args -> f args)))

            // Add our own attributes. They must have unique names which must match the names below.
            match source with None -> () | Some v -> attribs.Add (CachedImageSourceAttribKey, v)
            match aspect with None -> () | Some v -> attribs.Add (AspectAttribKey, v)
            match isOpaque with None -> () | Some v -> attribs.Add (IsOpaqueAttribKey, v)
            match loadingPlaceholder with None -> () | Some v -> attribs.Add (CachedImageLoadingPlaceholderAttribKey, v)
            match errorPlaceholder with None -> () | Some v -> attribs.Add (CachedImageErrorPlaceholderAttribKey, v)
            match cacheType with None -> () | Some v -> attribs.Add (CachedImageCacheTypeAttribKey, v)
            match cacheDuration with None -> () | Some v -> attribs.Add (CachedImageCacheDurationAttribKey, v)
            match cacheKeyFactory with None -> () | Some v -> attribs.Add (CachedImageCacheKeyFactoryAttribKey, v)
            match loadingDelay with None -> () | Some v -> attribs.Add (CachedImageLoadingDelayAttribKey, v)
            match loadingPriority with None -> () | Some v -> attribs.Add (CachedImageLoadingPriorityAttribKey, v)
            match customDataResolver with None -> () | Some v -> attribs.Add (CachedImageCustomDataResolverAttribKey, v)
            match retryCount with None -> () | Some v -> attribs.Add (CachedImageRetryCountAttribKey, v)
            match retryDelay with None -> () | Some v -> attribs.Add (CachedImageRetryDelayAttribKey, v)
            match downsampleWidth with None -> () | Some v -> attribs.Add (CachedImageDownsampleWidthAttribKey, v)
            match downsampleHeight with None -> () | Some v -> attribs.Add (CachedImageDownsampleHeightAttribKey, v)
            match downsampleToViewSize with None -> () | Some v -> attribs.Add (CachedImageDownsampleToViewSizeAttribKey, v)
            match downsampleUseDipUnits with None -> () | Some v -> attribs.Add (CachedImageDownsampleUseDipUnitsAttribKey, v)
            match fadeAnimationEnabled with None -> () | Some v -> attribs.Add (CachedImageFadeAnimationEnabledAttribKey, v)
            match fadeAnimationDuration with None -> () | Some v -> attribs.Add (CachedImageFadeAnimationDurationAttribKey, v)
            match fadeAnimationForCachedImages with None -> () | Some v -> attribs.Add (CachedImageFadeAnimationForCachedImagesAttribKey, v)
            match bitmapOptimizations with None -> () | Some v -> attribs.Add (CachedImageBitmapOptimizationsAttribKey, v)
            match invalidateLayoutAfterLoaded with None -> () | Some v -> attribs.Add (CachedImageInvalidateLayoutAfterLoadedAttribKey, v)
            match transformPlaceholders with None -> () | Some v -> attribs.Add (CachedImageTransformPlaceholdersAttribKey, v)
            match transformations with None -> () | Some v -> attribs.Add (CachedImageTransformationsAttribKey, v)
            match downloadProgress with None -> () | Some v -> attribs.Add (CachedImageDownloadProgressAttribKey, v)
            match downloadStarted with None -> () | Some v -> attribs.Add (CachedImageDownloadStartedAttribKey, v)
            match fileWriteFinished with None -> () | Some v -> attribs.Add (CachedImageFileWriteFinishedAttribKey, v)
            match finish with None -> () | Some v -> attribs.Add (CachedImageFinishAttribKey, v)
            match success with None -> () | Some v -> attribs.Add (CachedImageSuccessAttribKey, v)
            match error with None -> () | Some v -> attribs.Add (CachedImageErrorAttribKey, v)
    
            // The incremental update method
            let updater1 = ViewExtensions.PrimitiveUpdater(source, (fun (target: CachedImage) v -> target.Source <- ViewConverters.convertFabulousImageToXamarinFormsImageSource v))
            let updater2 = ViewExtensions.PrimitiveUpdater(aspect, (fun (target: CachedImage) v -> target.Aspect <- v))
            let updater3 = ViewExtensions.PrimitiveUpdater(isOpaque, (fun (target: CachedImage) v -> target.IsOpaque <- v))
            let updater4 = ViewExtensions.PrimitiveUpdater(loadingPlaceholder, (fun (target: CachedImage) v -> target.LoadingPlaceholder <- ViewConverters.convertFabulousImageToXamarinFormsImageSource v))
            let updater5 = ViewExtensions.PrimitiveUpdater(errorPlaceholder, (fun (target: CachedImage) v -> target.ErrorPlaceholder <- ViewConverters.convertFabulousImageToXamarinFormsImageSource v))
            let updater6 = ViewExtensions.PrimitiveUpdater(cacheType, (fun (target: CachedImage) v -> target.CacheType <- Option.toNullable v))
            let updater7 = ViewExtensions.PrimitiveUpdater(cacheDuration, (fun (target: CachedImage) v -> target.CacheDuration <- Option.toNullable v))
            let updater8 = ViewExtensions.PrimitiveUpdater(cacheKeyFactory, (fun (target: CachedImage) v -> target.CacheKeyFactory <- v))
            let updater9 = ViewExtensions.PrimitiveUpdater(loadingDelay, (fun (target: CachedImage) v -> target.LoadingDelay <- Option.toNullable v))
            let updater10 = ViewExtensions.PrimitiveUpdater(loadingPriority, (fun (target: CachedImage) v -> target.LoadingPriority <- v))
            let updater11 = ViewExtensions.PrimitiveUpdater(customDataResolver, (fun (target: CachedImage) v -> target.CustomDataResolver <- v))
            let updater12 = ViewExtensions.PrimitiveUpdater(retryCount, (fun (target: CachedImage) v -> target.RetryCount <- v))
            let updater13 = ViewExtensions.PrimitiveUpdater(retryDelay, (fun (target: CachedImage) v -> target.RetryDelay <- v))
            let updater14 = ViewExtensions.PrimitiveUpdater(downsampleWidth, (fun (target: CachedImage) v -> target.DownsampleWidth <- v))
            let updater15 = ViewExtensions.PrimitiveUpdater(downsampleHeight, (fun (target: CachedImage) v -> target.DownsampleHeight <- v))
            let updater16 = ViewExtensions.PrimitiveUpdater(downsampleToViewSize, (fun (target: CachedImage) v -> target.DownsampleToViewSize <- v))
            let updater17 = ViewExtensions.PrimitiveUpdater(downsampleUseDipUnits, (fun (target: CachedImage) v -> target.DownsampleUseDipUnits <- v))
            let updater18 = ViewExtensions.PrimitiveUpdater(fadeAnimationEnabled, (fun (target: CachedImage) v -> target.FadeAnimationEnabled <- v))
            let updater19 = ViewExtensions.PrimitiveUpdater(fadeAnimationDuration, (fun (target: CachedImage) v -> target.FadeAnimationDuration <- v))
            let updater20 = ViewExtensions.PrimitiveUpdater(fadeAnimationForCachedImages, (fun (target: CachedImage) v -> target.FadeAnimationForCachedImages <- v))
            let updater21 = ViewExtensions.PrimitiveUpdater(bitmapOptimizations, (fun (target: CachedImage) v -> target.BitmapOptimizations <- v))
            let updater22 = ViewExtensions.PrimitiveUpdater(invalidateLayoutAfterLoaded, (fun (target: CachedImage) v -> target.InvalidateLayoutAfterLoaded <- v))
            let updater23 = ViewExtensions.PrimitiveUpdater(transformPlaceholders, (fun (target: CachedImage) v -> target.TransformPlaceholders <- v))
            let updater24 = ViewExtensions.ElementCollectionUpdater(transformations)
            let updater25 = ViewExtensions.EventUpdater(downloadStarted)
            let updater26 = ViewExtensions.EventUpdater(downloadProgress)
            let updater27 = ViewExtensions.EventUpdater(fileWriteFinished)
            let updater28 = ViewExtensions.EventUpdater(finish)
            let updater29 = ViewExtensions.EventUpdater(success)
            let updater30 = ViewExtensions.EventUpdater(error)

            // The update method
            let update token (target: CachedImage) = 
                viewUpdater token target
                updater1 token target
                updater2 token target
                updater3 token target
                updater4 token target
                updater5 token target
                updater6 token target
                updater7 token target
                updater8 token target
                updater9 token target
                updater10 token target
                updater11 token target
                updater12 token target
                updater13 token target
                updater14 token target
                updater15 token target
                updater16 token target
                updater17 token target
                updater18 token target
                updater19 token target
                updater20 token target
                updater21 token target
                updater22 token target
                updater23 token target
                updater24 token target.Transformations
                updater25 token target.DownloadStarted
                updater26 token target.DownloadProgress
                updater27 token target.FileWriteFinished
                updater28 token target.Finish
                updater29 token target.Success
                updater30 token target.Error
                
            // Create a ViewElement with the instruction to create and update a CachedImage
            ViewElement.Create(CachedImage, update, attribs.Close())
            
#if DEBUG
    let sample =
        View.CachedImage(
            source = c (Path "path/to/image.png"),
            loadingPlaceholder = c (Path "path/to/loading-placeholder.png"),
            errorPlaceholder = c (Path "path/to/error-placeholder.png"),
            height = c 600.,
            width = c 600.
        )
#endif